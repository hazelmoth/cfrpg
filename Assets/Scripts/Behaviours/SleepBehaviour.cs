using SettlementSystem;
using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;

public class SleepBehaviour : IAiBehaviour
{
    private SettlementManager settlement;
    private Actor actor;
    private IAiBehaviour navigationBehaviour;
    private IBed bed;
    private Vector2 outOfBedPosition;
    private Coroutine sleepCoroutine;

    public SleepBehaviour (Actor actor)
    {
        this.actor = actor;
        settlement = GameObject.FindObjectOfType<SettlementManager>();
        IsRunning = false;
    }

    public bool IsRunning { get; private set; }

    public void Cancel()
    {
        if (!IsRunning) return;

        IsRunning = false;
        navigationBehaviour?.Cancel();
        if (sleepCoroutine != null)
        {
            actor.StopCoroutine(sleepCoroutine);
        }
        if (actor.GetData().PhysicalCondition.Sleeping)
        {
            actor.transform.position = outOfBedPosition;
        }
        actor.GetData().PhysicalCondition.WakeUp();
    }

    public void Execute()
    {
        IsRunning = true;
        House house = settlement.GetHouse(actor.ActorId);
        if (house == null)
        {
            HandleNoBed();
            return;
        }
        string scene = house.GetComponentInChildren<ScenePortal>().DestinationSceneObjectId;
        bed = SceneObjectManager.GetSceneObjectFromId(scene).GetComponentInChildren<IBed>();
        if (bed == null)
        {
            HandleNoBed();
            return;
        }

        navigationBehaviour = new NavigateNextToObjectBehaviour(actor, ((MonoBehaviour)bed).gameObject, scene, NavFinished);
        navigationBehaviour.Execute();


        void HandleNoBed()
        {
            Debug.Log("Settler is missing a bed!?", actor);
            navigationBehaviour?.Cancel();
            IsRunning = false;
        }

        void NavFinished(bool success)
        {
            if (success)
            {
                sleepCoroutine = actor.StartCoroutine(SleepCoroutine());
            }
            else
            {
                Cancel();
            }
        }
    }

    private IEnumerator SleepCoroutine()
    {
        outOfBedPosition = actor.transform.position;
        Vector2 sleepPosition = bed.SleepPositionWorldCoords;

        actor.GetData().PhysicalCondition.Sleep(bed);
        actor.transform.position = sleepPosition;

        while (actor.GetData().PhysicalCondition.Sleeping)
        {
            yield return null;
        }
        Cancel();
    }
}
