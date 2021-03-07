using ContentLibraries;
using Items;
using UnityEngine;

public class PlayerEquipmentController : MonoBehaviour
{
	// Update is called once per frame
    void Update()
    {
		if (PauseManager.Paused)
		{
			return;
		}

		Actor player = GetPlayer();
		if (player == null)
		{
			return;
		}

	    ActorEquipmentHandler equipManager = player.GetComponent<ActorEquipmentHandler>();

        ActorRace race = ContentLibrary.Instance.Races.GetById(player.GetData().Race);
        Vector2 heldItemPosition = (Vector2) player.transform.position + race.GetItemPosition(player.Direction) * Vector2.up;
	    float angleToMouse = MousePositionHelper.AngleToMouse(heldItemPosition);
	    equipManager.SetEquipmentAngle(angleToMouse);

	    if (Input.GetMouseButtonDown(0))
	    {
            equipManager.ActivateAimedEquipment(TileMouseInputManager.GetTileUnderCursor(player.CurrentScene));
	    }

		// Logic for placing tile marker when holding a seed bag

		ItemStack equipped = player.GetData().Inventory.GetEquippedItem();
		if (equipped != null && equipped.GetData() is IPloppable ploppable && ploppable.VisibleTileSelector(equipped))
		{
			string scene = player.CurrentScene;
			Vector2 pos = player.transform.position;
			Vector2 targetPos = pos + player.Direction.ToVector2() * 0.75f;

			TileMarkerController.SetTileMarker(targetPos.ToVector2Int());
		}
		else
		{
			if (equipped as ITileSelectable == null) 
				TileMarkerController.HideTileMarkers();
		}
	}

    private Actor GetPlayer()
    {
	    return ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
    }
}
