using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour {

	private Item currentEquippedItem;
	private HumanSpriteController spriteController;

	// Use this for initialization
	void Start () 
	{
		if (spriteController == null)
		{
			spriteController = GetComponent<HumanSpriteController>();
		}
		HotbarManager.OnHotbarSlotSelected += OnItemEquipped;
		TileMouseInputManager.OnTileClicked += OnItemUse;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (currentEquippedItem != null && currentEquippedItem is Gun gun)
		{
			ActorRace playerRace = ContentLibrary.Instance.Races.GetById(ActorRegistry.Get(PlayerController.PlayerActorId).data.Race);
			Vector2 gunPos = playerRace.GetItemPosition(ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.Direction) + (Vector2)ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.transform.position;
			float angle = MousePositionHelper.AngleToMouse(gunPos);
			EquipmentRenderer.PointItem(ActorRegistry.Get(PlayerController.PlayerActorId).gameObject, gun, angle, true);
			spriteController.FaceTowardsMouse = true;

			if (Input.GetMouseButtonDown(0))
			{
				angle += (Random.value * gun.spread) - (gun.spread / 2);
				Vector2 projectileOrigin = (Vector2)ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.SpritesObject.transform.position +
				                           ContentLibrary.Instance.Races.GetById(ActorRegistry.Get(PlayerController.PlayerActorId).data.Race)
					                           .GetItemPosition(ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.Direction);

				if (ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.Direction == Direction.Right)
				{
					projectileOrigin += (Vector2)(Quaternion.AngleAxis(angle, Vector3.forward) * gun.projectileOffset);
				}
				else
				{
					projectileOrigin += (Vector2)(Quaternion.AngleAxis(180 - angle, Vector3.forward) *
					                              gun.projectileOffset * (Vector2.left + Vector2.up));
				}

				bool flipProjectile = ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.Direction != Direction.Right;

				Collider2D playerCollider = ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.GetComponent<Collider2D>();

				ProjectileSystem.LaunchProjectile(
					gun.projectile,
					projectileOrigin,
					angle,
					gun.velocity,
					gun.damage,
					gun.range,
					gun.projectileRadius,
					playerCollider,
					flipProjectile);
			}
		}
		else
		{
			EquipmentRenderer.StopRendering(ActorRegistry.Get(PlayerController.PlayerActorId).gameObject);
			spriteController.FaceTowardsMouse = false;
		}
	}

	void OnItemEquipped (int index) 
	{
		currentEquippedItem = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetHotbarArray() [index];
		PointableItem equippedEquippable = currentEquippedItem as PointableItem;
		if (equippedEquippable != null) {
			TileMouseInputManager.SetMaxDistance (equippedEquippable.TileSelectorRange);
			TileMouseInputManager.SetCheckingForInput (equippedEquippable.UseTileSelector);
		} else {
			TileMouseInputManager.SetCheckingForInput (false);
		}
	}

	void OnItemUse (Vector3Int tilePos) 
	{
		PointableItem equippedEquippable = currentEquippedItem as PointableItem;
		if (equippedEquippable != null) {
			equippedEquippable.Activate (tilePos);
		}
		SwingableItem equippedSwingable = currentEquippedItem as SwingableItem;
		if (equippedSwingable != null)
		{
			// VVV Called from PlayerAttackHandler?
			//equippedSwingable.Swing(GetComponent<Actor>());
		}
	}
}
