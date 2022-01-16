using Items;
using UnityEngine;

public class ActorAttackHandler : MonoBehaviour
{
	private Actor actor;
	private ActorPunchExecutor puncher;
	private ActorEquipmentHandler equipment;

	public void Attack()
	{
		if (actor == null)
			actor = GetComponent<Actor>();
		if (equipment == null)
			equipment = GetComponent<ActorEquipmentHandler>();

		ActorInventory inv = actor.GetData().Inventory;

		if (inv.EquippedItem?.GetData() is SwingableItem or IActivatable or IPloppable)
		{
			equipment.ActivateNonAimedEquipment();
		}
		else
		{
			// No activatable item is equipped; throw a punch instead.
			ThrowPunch();
		}
	}
	
	private void ThrowPunch ()
	{
		ThrowPunch(actor.Direction.ToVector2());
	}
	
	// Throws a punch in the given direction, performing an animation and causing
	// damage to any Actors located in that direction.
	public void ThrowPunch (Vector2 direction)
	{
		if (puncher == null)
		{
			puncher = GetComponent<ActorPunchExecutor>();
			if (puncher == null)
				puncher = gameObject.AddComponent<ActorPunchExecutor>();
		}

		puncher.InitiatePunch(direction);
	}
}
