using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A behaviour that makes an actor go outside and wander around.
public class GoForWalkBehaviour : IAiBehaviour
{
    private Actor actor;
    private IAiBehaviour currentBehaviour;

    public GoForWalkBehaviour (Actor actor)
    {
        this.actor = actor;
    }

    public bool IsRunning { get; private set; }

    public void Cancel()
    {
        if (!IsRunning) return;
        IsRunning = false;
        currentBehaviour?.Cancel();
    }

    public void Execute()
    {
        currentBehaviour?.Cancel();
        IsRunning = true;
        if (actor.CurrentScene != SceneObjectManager.WorldSceneId)
        {
            currentBehaviour = new NavigateToSceneBehaviour(actor, SceneObjectManager.WorldSceneId, OnNavigationFinished);
            currentBehaviour.Execute();
        }
        else
        {
            OnNavigationFinished(true);
        }
    }

    private void OnNavigationFinished (bool success)
    {
        if (!success)
        {
            Cancel();
            return;
        }

        currentBehaviour?.Cancel();
        currentBehaviour = new WanderBehaviour(actor);
        currentBehaviour.Execute();
    }
}
