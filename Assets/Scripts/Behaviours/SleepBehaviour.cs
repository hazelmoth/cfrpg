using SettlementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepBehaviour : IAiBehaviour
{
    private SettlementManager settlement;
    private Actor actor;
    private IAiBehaviour navigationBehaviour;

    public SleepBehaviour (Actor actor)
    {
        this.actor = actor;
        settlement = GameObject.FindObjectOfType<SettlementManager>();
        IsRunning = false;
    }

    public bool IsRunning { get; private set; }

    public void Cancel()
    {
        IsRunning = false;
        navigationBehaviour?.Cancel();
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
        IBed bed = SceneObjectManager.GetSceneObjectFromId(scene).GetComponentInChildren<IBed>();
        if (bed == null)
        {
            HandleNoBed();
            return;
        }

        navigationBehaviour = new NavigateNextToObjectBehaviour(actor, ((MonoBehaviour)bed).gameObject, scene, NavFinished);
        navigationBehaviour.Execute();


        void HandleNoBed()
        {
            Debug.Log("Settler is missing a bed!?");
            navigationBehaviour?.Cancel();
            IsRunning = false;
        }

        void NavFinished(bool success)
        {
            IsRunning = false;
        }
    }
}
