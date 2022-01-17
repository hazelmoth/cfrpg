using System.Collections;
using SettlementSystem;
using UnityEngine;

namespace AI.Behaviours
{
    public class SleepBehaviour : IAiBehaviour
    {
        private const float MaxSleepDist = 0.3f; // If an actor is too far from the bed, we'll wake them up
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
            if (actor.GetData().Health.Sleeping)
            {
                actor.transform.position = outOfBedPosition;
            }
            actor.GetData().Health.WakeUp();
        }

        public void Execute()
        {
            /*IsRunning = true;
            BuildingInfo buildingInfo = settlement.GetHouse(actor.ActorId);
            if (buildingInfo == null)
            {
                HandleNoBed();
                return;
            }
            string scene = buildingInfo.GetComponentInChildren<ScenePortal>().DestinationSceneObjectId;
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
                Debug.LogWarning("Settler is missing a bed!?", actor);
                Cancel();
            }

            void NavFinished(bool success)
            {
                if (!IsRunning) return; // If this behaviour's already been cancelled, nothing else should happen.

                if (success)
                {
                    navigationBehaviour?.Cancel();
                    navigationBehaviour = null;

                    if (sleepCoroutine != null) { actor.StopCoroutine(sleepCoroutine); }
                    sleepCoroutine = actor.StartCoroutine(SleepCoroutine());
                }
                else
                {
                    Debug.LogWarning("A settler failed to navigate to their bed!", (MonoBehaviour)bed);
                    Cancel();
                }
            }*/
        }

        private IEnumerator SleepCoroutine()
        {
            outOfBedPosition = actor.transform.position;
            Vector2 sleepPosition = bed.SleepPositionWorldCoords;

            actor.GetData().Health.Sleep(bed);
            actor.transform.position = sleepPosition;

            while (actor.GetData().Health.Sleeping && Vector2.Distance(actor.transform.position, bed.SleepPositionWorldCoords) <= MaxSleepDist)
            {
                yield return null;
            }
            Cancel();
        }
    }
}
