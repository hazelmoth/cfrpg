using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAttackHandler : MonoBehaviour
{
	private Actor actor;
	private ActorPunchExecutor puncher;

	public void TriggerAttackInput()
	{
		if (actor == null)
			actor = GetComponent<Actor>();

		ActorInventory inv = actor.GetData().Inventory;

		if (inv.GetEquippedItem() != null)
		{
			SwingableItem equippedSwingable = inv.GetEquippedItem().GetData() as SwingableItem;
			if (equippedSwingable != null)
			{
				equippedSwingable.Swing(actor);
				return;
			}
			else if (inv.GetEquippedItem() is IPloppable ploppable)
			{
				string scene = actor.CurrentScene;
				Vector2 pos = actor.transform.position;
				pos = TilemapInterface.WorldPosToScenePos(pos, scene);
				Vector2 targetPos = pos + actor.Direction.ToVector2();

				TileLocation target = new TileLocation(Vector2Int.FloorToInt(targetPos), scene);
				ploppable.Use(target);
			}
		}
		else
		{
			// If no item is equipped, throw a punch instead
			if (puncher == null)
			{
				puncher = GetComponent<ActorPunchExecutor>();
				if (puncher == null)
					puncher = gameObject.AddComponent<ActorPunchExecutor>();
			}

			puncher.InitiatePunch(actor.Direction.ToVector2());
		}
	}
}
