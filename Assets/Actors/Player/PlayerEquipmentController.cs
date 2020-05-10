using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentController : MonoBehaviour
{
	// Update is called once per frame
    void Update()
    {
	    ActorEquipmentManager equipManager = GetPlayer().GetComponent<ActorEquipmentManager>();

        ActorRace race = ContentLibrary.Instance.Races.GetById(GetPlayer().GetData().Race);
        Vector2 gunPos = (Vector2) GetPlayer().transform.position + race.GetItemPosition(GetPlayer().Direction) * Vector2.up;
	    float angle = MousePositionHelper.AngleToMouse(gunPos);
	    equipManager.SetEquipmentAngle(angle);

	    if (Input.GetMouseButtonDown(0))
	    {
            equipManager.ActivateEquipment();
	    }
    }

    private Actor GetPlayer()
    {
	    return ActorRegistry.Get(PlayerController.PlayerActorId).gameObject;
    }
}
