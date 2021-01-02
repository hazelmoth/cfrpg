using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A behaviour that encompasses all the actions of a member of the player's settlement.
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

            float time = TimeKeeper.TimeAsFraction;
            if (time > sleepStart || time < sleepEnd)
            {
                if (SleepBehaviourRunning) continue;
                currentBehaviour?.Cancel();
                currentBehaviour = new SleepBehaviour(actor);
                currentBehaviour.Execute();
                continue;
            }
            else
            {
                // Nothing to do. Act on a whim!
                int currentIndex = Mathf.FloorToInt(CalculateWhim() * casualBehaviours.Count);
                if (SleepBehaviourRunning || currentIndex != lastIndex)
                {
                    currentBehaviour?.Cancel();
                    currentBehaviour = CreateBehaviour(casualBehaviours[currentIndex]);
                    currentBehaviour.Execute();
                    lastIndex = currentIndex;
                }
            }
        }
    }

    private bool SleepBehaviourRunning { get
        {
            if (currentBehaviour == null) return false;
            if (currentBehaviour.GetType() != typeof(SleepBehaviour)) return false;
            if (!currentBehaviour.IsRunning) return false;
            return true;
        } }

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
        if (result >= 1f) { result -= 1f; }
        return result;
    }
}
