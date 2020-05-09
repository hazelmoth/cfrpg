using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages which item this actor currently has equipped, and what direction it's being pointed in.
public class ActorEquipmentManager : MonoBehaviour {

	private Item currentEquippedItem;
	private Actor thisActor;
	private HumanSpriteController spriteController;
	private float angle;

	// Use this for initialization
	private void Start () 
	{
		if (spriteController == null)
		{
			spriteController = GetComponent<HumanSpriteController>();
		}

		thisActor = GetComponent<Actor>();
	}

	private void Update()
	{
		if (currentEquippedItem != null && currentEquippedItem is Gun gun)
		{
			EquipmentRenderer.PointItem(thisActor, gun, angle, true);
			spriteController.HoldDirection(DirectionMethods.AngleToDir(angle));
		}
		else
		{
			EquipmentRenderer.StopRendering(thisActor);
			spriteController.StopHoldingDirection();
		}
	}

	// Changes the angle that this actor's current equipment is pointing, if applicable
	public void SetEquipmentAngle(float angle)
	{
		this.angle = angle;
	}

	public void ActivateEquipment()
	{
		if (currentEquippedItem != null && currentEquippedItem is Gun gun)
		{
			float shotAngle = angle + (Random.value * gun.spread) - (gun.spread / 2);

			Vector2 projectileOrigin = (Vector2) thisActor
				                           .SpritesObject.transform.position + 
			                           ContentLibrary.Instance.Races
				                           .GetById(thisActor.GetData().Race)
				                           .GetItemPosition(thisActor.Direction);

			if (thisActor.Direction == Direction.Right)
			{
				projectileOrigin += (Vector2) (Quaternion.AngleAxis(shotAngle, Vector3.forward) * gun.projectileOffset);
			}
			else
			{
				projectileOrigin += (Vector2) (Quaternion.AngleAxis(180 - shotAngle, Vector3.forward) *
				                               gun.projectileOffset * (Vector2.left + Vector2.up));
			}

			bool flipProjectile = thisActor.Direction != Direction.Right;

			Collider2D playerCollider = thisActor.GetComponent<Collider2D>();

			ProjectileSystem.LaunchProjectile(
				gun.projectile,
				projectileOrigin,
				shotAngle,
				gun.velocity,
				gun.damage,
				gun.range,
				gun.projectileRadius,
				playerCollider,
				flipProjectile);
		}
	}

	public void EquipItem (Item item)
	{
		currentEquippedItem = item;
		PointableItem equippedEquippable = currentEquippedItem as PointableItem;
		if (equippedEquippable != null) {
			TileMouseInputManager.SetMaxDistance (equippedEquippable.TileSelectorRange);
			TileMouseInputManager.SetCheckingForInput (equippedEquippable.UseTileSelector);
		} else {
			TileMouseInputManager.SetCheckingForInput (false);
		}
	}

	public Item GetEquipped()
	{
		return currentEquippedItem;
	}
}
