using UnityEngine;
using TMPro;

public class InteractionTextController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text = null;
	private PickupDetector detector = null;

    // Update is called once per frame
    private void Update()
    {
		if (detector == null) {
			if (ActorRegistry.Get(PlayerController.PlayerActorId) == null)
				return;
			detector = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.GetComponent<PickupDetector> ();
		}

		IPickuppable currentDetectedObject = detector.GetCurrentDetectedItem ();

		if (currentDetectedObject != null)
		{
			ItemStack item = currentDetectedObject.ItemPickup;
			text.text = "Pick up " + item.GetData().GetItemName(item.GetModifiers());
			if (currentDetectedObject.ItemPickup.quantity > 1)
			{
				text.text += " (" + currentDetectedObject.ItemPickup.quantity + ")";
			}
		}
		else {
			text.text = null;
		}
    }
		
}
