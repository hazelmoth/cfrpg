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
        if (!IsRunning) return;

        if (coroutine != null) actor.StopCoroutine(coroutine);
        if (navBehaviour != null && navBehaviour.IsRunning) navBehaviour.Cancel();
    }

    public void Execute()
    {
        if (IsRunning) Cancel();

        if (FindAvailableProjects().Count == 0)
        {
            Debug.LogWarning("Started a BuilderWorkBehaviour with no projects available.", actor);
            return;
        }

        coroutine = actor.StartCoroutine(Coroutine());
        IsRunning = true;
    }

    private IEnumerator Coroutine()
    {
        while (true)
        {
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
            navBehaviour = new NavigateNextToObjectBehaviour(actor, project.gameObject, projectScene, ((bool result) => { navFinished = true; navSucceeded = result; } ));
            navBehaviour.Execute();

            while (!navFinished) yield return null;

            if (!navSucceeded)
            {
                Cancel();
                yield break;
            }
            Debug.Assert(navBehaviour == null || !navBehaviour.IsRunning, "NavBehaviour still running after it finished!", actor);

            // We have ostensibly reached the construction site.
            while (project != null && project.Work < project.TotalWorkNeeded && DistanceCheck(project))
            {
                project.AddWork(Time.deltaTime); // Add one work per second
                yield return null;
            }
            // Repeat ad infinitum.
        }
    }

    private bool DistanceCheck (ConstructionSite project)
    {
        float maxDist = 1.3f;
        EntityData ent = ContentLibrary.Instance.Entities.Get(project.GetComponent<EntityObject>().EntityId);

        foreach (Vector2 pos in ent.baseShape)
        {
            Vector2 tileCenter = project.transform.position.ToVector2() + pos + new Vector2(0.5f, 0.5f);
            float distToActor = Vector2.Distance(actor.transform.position, tileCenter);
            if (distToActor + 0.5f < maxDist) return true;
        }
        return false;
    }

    private ConstructionSite ChooseBest (IEnumerable<ConstructionSite> projects)
    {
        return projects.Single(); // TODO pick a project based on distance, or something
                                  // and maybe take into account which ones are being worked on
    }

    public static List<ConstructionSite> FindAvailableProjects ()
    {
        List<ConstructionSite> result = new List<ConstructionSite>();
        foreach (ConstructionSite project in GameObject.FindObjectsOfType<ConstructionSite>())
        {
            if (project.Work < project.TotalWorkNeeded) result.Add(project);
        }
        return result;
    }
}
