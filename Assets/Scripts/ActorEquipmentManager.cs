using Items;
using UnityEngine;

// Manages which item this actor currently has equipped, and what direction it's being pointed in.
public class ActorEquipmentManager : MonoBehaviour {

	private ItemData currentEquippedItem;
	private Actor thisActor;
	private ActorSpriteController spriteController;
	private float angle;

	// Use this for initialization
	private void Start () 
	{
		if (spriteController == null)
		{
			spriteController = GetComponent<ActorSpriteController>();
		}

		thisActor = GetComponent<Actor>();
	}

	private void Update()
	{
		if (PauseManager.GameIsPaused)
		{
			return;
		}
		if (currentEquippedItem != null && currentEquippedItem is IAimable item)
		{
			EquipmentRenderer.RenderItem(thisActor, item, angle, true);
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
		if (currentEquippedItem == null)
		{
			return;
		}

		if (currentEquippedItem is IGun gun)
		{
			float shotAngle = angle + (Random.value * gun.Spread) - (gun.Spread / 2);

			Vector2 projectileOrigin = (Vector2)thisActor
										   .SpritesObject.transform.position +
									   ContentLibrary.Instance.Races
										   .GetById(thisActor.GetData().Race)
										   .GetItemPosition(thisActor.Direction);

			if (thisActor.Direction == Direction.Right)
			{
				projectileOrigin += (Vector2)(Quaternion.AngleAxis(shotAngle, Vector3.forward) * gun.ProjectileOffset);
			}
			else
			{
				projectileOrigin += (Vector2)(Quaternion.AngleAxis(180 - shotAngle, Vector3.forward) *
											   gun.ProjectileOffset * (Vector2.left + Vector2.up));
			}

			bool flipProjectile = thisActor.Direction != Direction.Right;

			Collider2D playerCollider = thisActor.GetComponent<Collider2D>();

			ProjectileSystem.LaunchProjectile(
				gun.Projectile,
				projectileOrigin,
				shotAngle,
				gun.Velocity,
				gun.Damage,
				gun.Range,
				gun.ProjectileRadius,
				playerCollider,
				flipProjectile);
		}

		if (currentEquippedItem is IThrustWeapon weapon)
		{
			Vector2 forceOrigin = (Vector2)thisActor.SpritesObject.transform.position;
			forceOrigin = TilemapInterface.WorldPosToScenePos(forceOrigin, thisActor.CurrentScene);
			Vector2 dir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
			PunchSystem.ExertDirectionalPunch(forceOrigin, dir, weapon.WeaponRange, weapon.WeaponForce, thisActor.CurrentScene);

			EquipmentRenderer.ThrustItem(thisActor, weapon.ThrustDistance, weapon.ThrustDuration);
		}

		if (currentEquippedItem is ITileSelectable item)
		{
			TileLocation tile = TileMouseInputManager.GetTileUnderCursor(thisActor.CurrentScene);
			if (TileMouseInputManager.InRange)
			{
				item.Use(tile);
			}
		}
		
	}

	public void EquipItem (ItemData item)
	{
		currentEquippedItem = item;

		if (currentEquippedItem is ITileSelectable tileSelectableEquipment)
		{
			TileMouseInputManager.SetMaxDistance(tileSelectableEquipment.TileSelectorRange);
			TileMouseInputManager.SetCheckingForInput(tileSelectableEquipment.VisibleTileSelector);
		}
		else
		{
			TileMouseInputManager.SetCheckingForInput(false);
		}
	}

	public ItemData GetEquipped()
	{
		return currentEquippedItem;
	}
}
