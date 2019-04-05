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
			if (Player.instance == null)
				return;
			detector = Player.instance.GetComponent<DroppedItemDetector> ();
		}
		DroppedItem currentDetectedObject = detector.GetCurrentDetectedItem ();
		if (currentDetectedObject != null)
		{
			text.text = "Pick up " + ItemManager.GetItemById (currentDetectedObject.ItemId).GetItemName ();
		}
		else {
			text.text = null;
		}
    }
		
}
