using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A purely systematic class; works across all actors
public static class ActorEatingSystem
{
	public delegate void EatEvent(Actor actor, Item item);
    public static event EatEvent OnItemEaten;


	public static bool AttemptEat (Actor actor, Item item)
    {
		ActorPhysicalCondition physCondition = actor.PhysicalCondition;

        if (item == null)
        {
			Debug.LogWarning("Someone just tried to eat a null item, for some reason.");
            return false;
        }
		if (physCondition == null)
		{
			Debug.LogWarning("Can't eat; actor missing ActorPhysicalCondition script!");
			return false;
		}

		Eat(actor, physCondition, item);
        return true;
    }

	static void Eat (Actor actor, ActorPhysicalCondition actorCondition, Item item)
    {
		
		actorCondition.IntakeNutrition(item.NutritionalValue);
		OnItemEaten?.Invoke(actor, item);
	}
}
