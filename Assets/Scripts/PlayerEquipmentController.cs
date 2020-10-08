using Items;
using UnityEngine;

public class PlayerEquipmentController : MonoBehaviour
{
	// Update is called once per frame
    void Update()
    {
	    ActorEquipmentManager equipManager = GetPlayer().GetComponent<ActorEquipmentManager>();

        ActorRace race = ContentLibrary.Instance.Races.GetById(GetPlayer().GetData().Race);
        Vector2 heldItemPosition = (Vector2) GetPlayer().transform.position + race.GetItemPosition(GetPlayer().Direction) * Vector2.up;
	    float angleToMouse = MousePositionHelper.AngleToMouse(heldItemPosition);
	    equipManager.SetEquipmentAngle(angleToMouse);

	    if (Input.GetMouseButtonDown(0))
	    {
            equipManager.ActivateEquipment();
	    }

		// Logic for placing tile marker when holding a seed bag

		ItemStack equipped = GetPlayer().GetData().Inventory.GetEquippedItem();
		if (equipped != null && equipped.GetData() is IPloppable)
		{
			string scene = GetPlayer().CurrentScene;
			Vector2 pos = GetPlayer().transform.position;
			Vector2 targetPos = pos + GetPlayer().Direction.ToVector2() * 0.75f;

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
