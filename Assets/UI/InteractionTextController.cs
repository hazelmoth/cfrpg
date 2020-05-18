using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionTextController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text = null;
	private DroppedItemDetector detector = null;

    // Update is called once per frame
    private void Update()
    {
		if (detector == null) {
			if (ActorRegistry.Get(PlayerController.PlayerActorId) == null)
				return;
			detector = ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.GetComponent<DroppedItemDetector> ();
		}
		DroppedItem currentDetectedObject = detector.GetCurrentDetectedItem ();
		if (currentDetectedObject != null)
		{
			text.text = "Pick up " + ContentLibrary.Instance.Items.Get (currentDetectedObject.ItemId).ItemName;
		}
		else {
			text.text = null;
		}
    }
		
}
