using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuilderWorkBehaviour : IAiBehaviour
{
    private Actor actor;
    private Coroutine coroutine;
    private IAiBehaviour navBehaviour;

    public bool IsRunning { get; private set; }

    public BuilderWorkBehaviour(Actor actor)
    {
        this.actor = actor;
        IsRunning = false;
    }

    public void Cancel()
    {
        Debug.Assert(IsRunning, "Tried to cancel behaviour that isn't running!", actor);

        IsRunning = false;
        if (coroutine != null) actor.StopCoroutine(coroutine);
        if (navBehaviour != null && navBehaviour.IsRunning) navBehaviour.Cancel();
    }

    public void Execute()
    {
        Debug.Assert(!IsRunning, "Tried to start already running behaviour!", actor);

        if (FindAvailableProjects().Count == 0)
        {
            Debug.LogWarning("Started a BuilderWorkBehaviour with no projects available.", actor);
            return;
        }

        IsRunning = true;
        coroutine = actor.StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine()
    {
        while (true)
        {
            Debug.Assert(IsRunning);

            // Find a project to work on.
            List<ConstructionSite> availableProjects = FindAvailableProjects();
            if (availableProjects.Count == 0)
            {   // No projects left.
                Cancel();
                yield break;
            }
            ConstructionSite project = ChooseBest(availableProjects);

            string projectScene = SceneObjectManager.GetSceneIdForObject(project.gameObject);
            bool navFinished = false;
            bool navSucceeded = false;

            // Navigate to that project.
            navBehaviour = new NavigateNextToObjectBehaviour(actor, project.gameObject, projectScene, ((bool result) => { navFinished = true; navSucceeded = result; } ));
            navBehaviour.Execute();

            while (!navFinished)
            {
                yield return null;
            }

            if (!navSucceeded)
            {
                Cancel();
                yield break;
            }
            else
            {
                if (!DistanceCheck(project, actor))
                {
                    Debug.LogWarning("Navigation succeeded, but distance check comes back false!\n" +
                        $"Naive entity-to-actor distance: {Vector2.Distance(actor.transform.position, project.transform.position)}", actor);
                    Cancel();
                    yield break;
                }
            }
            Debug.Assert(navBehaviour == null || !navBehaviour.IsRunning, $"NavBehaviour still running after it finished! Nav succeeded: {navSucceeded}", actor);

            if (!IsRunning) // This behaviour may have been cancelled during navigation
            {
                Cancel();
                yield break;
            }

            // We have ostensibly reached the construction site.
            while (project != null && project.Work < project.TotalWorkRequired && DistanceCheck(project, actor))
            {
                float oldWork = project.Work;
                project.AddWork(Time.deltaTime * 10); // Add 10 work per second
                Debug.Assert(project.Work > oldWork);

                yield return null;
            }
            // Repeat ad infinitum.
            yield return null;
        }
    }

    private static bool DistanceCheck (ConstructionSite project, Actor actor)
    {
        float maxDist = 1.3f;
        EntityData ent = ContentLibrary.Instance.Entities.Get(project.GetComponent<EntityObject>().EntityId);
        Debug.Assert(ent.entityId == "construction");

        Vector2 tileOffset = ent.pivotAtCenterOfTile ? Vector2.zero : new Vector2(0.5f, 0.5f);

        foreach (Vector2 pos in ent.baseShape)
        {
            Vector2 tileCenter = project.transform.position.ToVector2() + pos + tileOffset;
            float distToActor = Vector2.Distance(actor.transform.position, tileCenter);
            if (distToActor - 0.5f < maxDist) return true;
        }
        return false;
    }

    private ConstructionSite ChooseBest (IEnumerable<ConstructionSite> projects)
    {
        return (from proj in projects
                orderby Vector2.Distance(actor.transform.position, proj.transform.position)
                select proj).First();
    }

    public static List<ConstructionSite> FindAvailableProjects ()
    {
        List<ConstructionSite> result = new List<ConstructionSite>();
        foreach (ConstructionSite project in GameObject.FindObjectsOfType<ConstructionSite>())
        {
            if (project.Work < project.TotalWorkRequired) result.Add(project);
        }
        return result;
    }
}
