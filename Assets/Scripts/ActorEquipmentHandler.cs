using ActorAnim;
using ActorComponents;
using ContentLibraries;
using Items;
using UnityEngine;

// Methods for using whatever equipment an actor is holding
public class ActorEquipmentHandler : MonoBehaviour {

	private ItemStack currentEquippedItem;
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
		if (PauseManager.Paused)
		{
			return;
		}
		if (currentEquippedItem != null && currentEquippedItem.GetData() is IAimable item)
		{
			EquipmentRenderer.RenderItem(thisActor, item, angle, true);
			spriteController.LockDirection(DirectionMethods.AngleToDir(angle));
		}
		else
		{
			EquipmentRenderer.StopRendering(thisActor);
			spriteController.UnlockDirection();
		}
	}

	// Changes the angle that this thisActor's current equipment is pointing, if applicable
	public void SetEquipmentAngle(float angle)
	{
		this.angle = angle;
	}

	public void ActivateAimedEquipment(TileLocation target)
	{
		if (currentEquippedItem == null)
		{
			return;
		}

		if (currentEquippedItem.GetData() is IGun gun)
		{
			float shotAngle = angle + (Random.value * gun.Spread) - (gun.Spread / 2);

			Vector2 projectileOrigin = (Vector2)thisActor
										   .SpritesObject.transform.position +
									   ContentLibrary.Instance.Races
										   .Get(thisActor.GetData().RaceId)
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

		if (currentEquippedItem.GetData() is IThrustWeapon weapon)
		{
			Vector2 forceOrigin = thisActor.SpritesObject.transform.position;
			forceOrigin = TilemapInterface.WorldPosToScenePos(forceOrigin, thisActor.CurrentScene);
			Vector2 dir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
			ImpactSystem.ExertDirectionalForce(
				thisActor,
				forceOrigin,
				dir,
				weapon.WeaponRange,
				weapon.WeaponForce,
				weapon.DamageType,
				thisActor.CurrentScene);

			EquipmentRenderer.ThrustItem(thisActor, weapon.ThrustDistance, weapon.ThrustDuration);
		}

		if (currentEquippedItem.GetData() is ITileSelectable item)
		{
			if (TileMouseInputManager.InRange)
			{
				item.Use(target);
			}
		}
	}

	public void ActivateNonAimedEquipment()
	{
		if (currentEquippedItem == null) return;
		ItemData itemData = currentEquippedItem.GetData();

		if (itemData is IActivatable activatable)
		{
			activatable.Activate(currentEquippedItem.GetModifiers(), thisActor);
		}
		if (itemData is SwingableItem equippedSwingable)
		{
			equippedSwingable.Swing(thisActor);
		}
		if (itemData is IPloppable ploppable)
		{
			string scene = thisActor.CurrentScene;
			Vector2 pos = thisActor.transform.position;
			pos = TilemapInterface.WorldPosToScenePos(pos, scene);
			Vector2 targetPos = pos + thisActor.Direction.ToVector2();

			TileLocation target = new TileLocation(Vector2Int.FloorToInt(targetPos), scene);
			ItemStack newStack = ploppable.Use(target, currentEquippedItem);

			// Update the item in the inventory slot
			ActorInventory inv = thisActor.GetComponent<ActorInventory>();
			if (inv != null)
			{
				inv.SetItemInSlot(inv.EquippedHotbarSlot, InventorySlotType.Hotbar, newStack);
				currentEquippedItem = newStack;
			}
		}
	}

	public void SetEquippedItem (ItemStack item)
	{
		currentEquippedItem = item;

		if (currentEquippedItem != null && currentEquippedItem.GetData() is ITileSelectable tileSelectableEquipment)
		{
			TileMouseInputManager.SetMaxDistance(tileSelectableEquipment.TileSelectorRange);
			TileMouseInputManager.SetCheckingForInput(tileSelectableEquipment.VisibleTileSelector);
		}
		else
		{
			TileMouseInputManager.SetCheckingForInput(false);
		}
	}
}
