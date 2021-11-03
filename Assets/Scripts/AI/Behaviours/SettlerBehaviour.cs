using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A behaviour that encompasses all the actions of a member of the player's settlement.
namespace AI.Behaviours
{
    public class SettlerBehaviour : IAiBehaviour
    {
        private const float whimdexPeriod = 6000f; // in ticks
        private Actor actor;
        private List<Type> casualBehaviours;
        private IAiBehaviour currentBehaviour;
        private Coroutine coroutine;
        private int lastIndex = -1;
        private readonly float sleepStart = 0.9f;
        private readonly float sleepEnd = 0.25f;

        public SettlerBehaviour(Actor actor)
        {
            this.actor = actor;
            casualBehaviours = new List<Type>
            {
                typeof(GoForWalkBehaviour),
                typeof(ChillAtHomeBehaviour)
            };
        }

        public string CurrentSubBehaviourName => currentBehaviour.GetType().Name ?? "null";

        public bool IsRunning { get; private set; }

        public void Cancel()
        {
            IsRunning = false;

            currentBehaviour?.Cancel();
            currentBehaviour = null;
            if (coroutine != null) actor.StopCoroutine(coroutine);
        }

        public void Execute()
        {
            currentBehaviour?.Cancel();
            if (coroutine != null) actor.StopCoroutine(coroutine);

            coroutine = actor.StartCoroutine(BehaviourCoroutine());
            IsRunning = true;
        }

        private IEnumerator BehaviourCoroutine ()
        {
            while (true)
            {
                yield return null;

                if (PauseManager.Paused)
                {
                    continue;
                }

                float time = TimeKeeper.TimeOfDay;
                if (time > sleepStart || time < sleepEnd)
                {
                    if (AlreadyRunning(typeof(SleepBehaviour))) continue;
                    SwitchToBehaviour(typeof(SleepBehaviour));
                    lastIndex = -1;
                }
                else if (actor.GetData().Profession == Professions.BuilderProfessionID && BuilderWorkBehaviour.FindAvailableProjects().Count > 0)
                {
                    if (AlreadyRunning(typeof(BuilderWorkBehaviour))) continue;
                    SwitchToBehaviour(typeof(BuilderWorkBehaviour));
                    lastIndex = -1;
                }
                else
                {
                    // Nothing to do. Act on a whim!
                    int currentIndex = Mathf.FloorToInt(CalculateWhim() * casualBehaviours.Count);
                    if (currentIndex != lastIndex)
                    {
                        Type behaviourType = casualBehaviours[currentIndex];
                        SwitchToBehaviour(behaviourType);
                        lastIndex = currentIndex;
                    }
                }
            }
        }

        private void SwitchToBehaviour (Type behaviourType)
        {
            if (currentBehaviour != null && currentBehaviour.IsRunning) currentBehaviour.Cancel();
            currentBehaviour = CreateBehaviour(behaviourType);
            currentBehaviour.Execute();
        }

        private bool AlreadyRunning (Type behaviourType)
        {
            if (currentBehaviour == null) return false;
            if (currentBehaviour.GetType() != behaviourType) return false;
            if (!currentBehaviour.IsRunning) return false;
            return true;
        }

        // Instantiates a behaviour of the given type. Assumes the behaviour takes only an actor for its constructor.
        private IAiBehaviour CreateBehaviour (Type type)
        {
            object thing;
            thing = Activator.CreateInstance(type, new object[] { actor });
            return (IAiBehaviour)thing;
        }

        // A random number between 0 and 1 which fluctuates smoothly over time.
        private float CalculateWhim()
        {
            float result;
            result = Mathf.Sin((TimeKeeper.CurrentTick * 2 * Mathf.PI) / whimdexPeriod);
            result = Mathf.Clamp01(result);
            if (result == 1f) { result -= 0.001f; }
            return result;
        }
    }
}
