using Items;
using UnityEngine;

/// A purely systematic class; works across all actors.
public static class ActorEatingSystem
{
	/// Attempts to make the given actor eat one item of the given type.
	/// Returns false if eating is unsuccessful.
	public static bool AttemptEat (Actor actor, ItemData item)
    {
	    if (item == null)
        {
			Debug.LogWarning("Someone just tried to eat a null item, for some reason.");
            return false;
        }
	    if (!(item is IEdible))
	    {
		    Debug.LogWarning("Tried to eat inedible item: " + item.ItemId);
		    return false;
	    }

	    ((IEdible) item)!.ApplyEffects(actor.GetData());

        return true;
    }
}
