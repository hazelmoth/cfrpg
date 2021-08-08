using UnityEngine;

// A purely systematic class; works across all actors
public static class ActorEatingSystem
{
	public delegate void EatEvent(Actor actor, ItemData item);
    public static event EatEvent OnItemEaten;


	public static bool AttemptEat (Actor actor, ItemStack item)
    {
		ActorHealth physCondition = actor.GetData().Health;

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

	private static void Eat (Actor actor, ActorHealth actorCondition, ItemStack item)
    {
		
		actorCondition.IntakeNutrition(item.GetData().NutritionalValue);
		OnItemEaten?.Invoke(actor, item.GetData());
	}
}
