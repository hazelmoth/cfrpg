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

		if (inv.GetEquippedItem() != null && (inv.GetEquippedItem().GetData() is SwingableItem || inv.GetEquippedItem().GetData() is IPloppable))
		{
			equipment.ActivateNonAimedEquipment();
			return;
		}
		else
		{
			// No activatable item is equipped; throw a punch instead.
			ThrowPunch();
		}
	}

	private void ThrowPunch ()
	{
		if (puncher == null)
		{
			puncher = GetComponent<ActorPunchExecutor>();
			if (puncher == null)
				puncher = gameObject.AddComponent<ActorPunchExecutor>();
		}

		puncher.InitiatePunch(actor.Direction.ToVector2());
	}
}
