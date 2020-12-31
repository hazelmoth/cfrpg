using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A behaviour that encompasses all the actions of a member of the player's settlement
public class SettlerBehaviour : IAiBehaviour
{
    private const float whimdexPeriod = 6000f; // in ticks
    private Actor actor;
    private List<Type> casualBehaviours;
    private IAiBehaviour currentBehaviour;
    private Coroutine coroutine;
    private int lastIndex = -1;
    private float sleepStart = 0.9f;
    private float sleepEnd = 0.25f;
    private bool sleeping;

    public SettlerBehaviour(Actor actor)
    {
        this.actor = actor;
        casualBehaviours = new List<Type>();
        casualBehaviours.Add(typeof(WanderBehaviour));
        casualBehaviours.Add(typeof(ChillAtHomeBehaviour));
    }

    public bool IsRunning { get; private set; }

    public void Cancel()
    {
        IsRunning = false;
        currentBehaviour?.Cancel();
    }

    public void Execute()
    {
        IsRunning = true;
        coroutine = actor.StartCoroutine(BehaviourCoroutine());
    }

    private IEnumerator BehaviourCoroutine ()
    {
        while (true)
        {
            yield return null;

            float time = TimeKeeper.TimeAsFraction;
            if (time > sleepStart || time < sleepEnd)
            {
                if (sleeping) continue;
                currentBehaviour?.Cancel();
                currentBehaviour = new SleepBehaviour(actor);
                currentBehaviour.Execute();
                sleeping = true;
                continue;
            }
            sleeping = false;

            // Nothing to do. Act on a whim!
            int currentIndex = Mathf.FloorToInt(CalculateWhim() * casualBehaviours.Count);
            if (currentIndex != lastIndex)
            {
                currentBehaviour?.Cancel();
                currentBehaviour = CreateBehaviour(casualBehaviours[currentIndex]);
                currentBehaviour.Execute();
                lastIndex = currentIndex;
            }
        }
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
        if (result >= 1f) { result -= 1f; }
        return result;
    }
}
