using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionTextController : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI text = null;
	DroppedItemDetector detector = null;

    // Update is called once per frame
    void Update()
    {
		if (detector == null) {
			if (ActorRegistry.Get(PlayerController.PlayerActorId).gameObject == null)
				return;
			detector = ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.GetComponent<DroppedItemDetector> ();
		}
		DroppedItem currentDetectedObject = detector.GetCurrentDetectedItem ();
		if (currentDetectedObject != null)
		{
			text.text = "Pick up " + ContentLibrary.Instance.Items.GetItemById (currentDetectedObject.ItemId).GetItemName ();
		}
		else {
			text.text = null;
		}
    }
		
}
