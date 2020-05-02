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
			ActorRace playerRace = ContentLibrary.Instance.Races.GetById(Player.instance.Race);
			Vector2 gunPos = playerRace.GetItemPosition(Player.instance.Direction) + (Vector2)Player.instance.transform.position;
			float angle = MousePositionHelper.AngleToMouse(gunPos);
			EquipmentRenderer.PointItem(Player.instance, gun, angle, true);
			spriteController.FaceTowardsMouse = true;

			if (Input.GetMouseButtonDown(0))
			{
				angle += (Random.value * gun.spread) - (gun.spread / 2);
				Vector2 projectileOrigin = (Vector2)Player.instance.SpritesObject.transform.position +
				                           ContentLibrary.Instance.Races.GetById(Player.instance.Race)
					                           .GetItemPosition(Player.instance.Direction);

				if (Player.instance.Direction == Direction.Right)
				{
					projectileOrigin += (Vector2)(Quaternion.AngleAxis(angle, Vector3.forward) * gun.projectileOffset);
				}
				else
				{
					projectileOrigin += (Vector2)(Quaternion.AngleAxis(180 - angle, Vector3.forward) *
					                              gun.projectileOffset * (Vector2.left + Vector2.up));
				}

				bool flipProjectile = Player.instance.Direction != Direction.Right;

				Collider2D playerCollider = Player.instance.GetComponent<Collider2D>();

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
			EquipmentRenderer.StopRendering(Player.instance);
			spriteController.FaceTowardsMouse = false;
		}
	}

	void OnItemEquipped (int index) 
	{
		currentEquippedItem = Player.instance.Inventory.GetHotbarArray() [index];
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
