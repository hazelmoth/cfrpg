using UnityEngine;
using TMPro;

// Updates the interaction text display for items and any entities that support it.
public class InteractionTextController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text = null;
	private PickupDetector detector = null;
	private PlayerInteractionRaycaster raycaster = null;

    // Update is called once per frame
    private void LateUpdate()
    {
		text.text = "";
		if (PauseManager.Paused) return;
		
		if (detector == null) 
		{
			if (ActorRegistry.Get(PlayerController.PlayerActorId) == null) return;
			detector = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.GetComponent<PickupDetector> ();
			Debug.Assert(detector != null);
		}
		if (raycaster == null)
		{
			if (ActorRegistry.Get(PlayerController.PlayerActorId) == null) return;
			raycaster = GameObject.FindObjectOfType<PlayerInteractionRaycaster>();
			Debug.Assert(raycaster != null);
		}

		IPickuppable detectedPickuppable = detector.GetCurrentDetectedItem();
		GameObject detectedInteractable = raycaster.DetectInteractableObject();

		if (detectedPickuppable != null)
		{
			ItemStack item = detectedPickuppable.ItemPickup;
			text.text = "Pick up " + item.GetData().GetItemName(item.GetModifiers());
			if (detectedPickuppable.ItemPickup.Quantity > 1)
			{
				text.text += " (" + detectedPickuppable.ItemPickup.Quantity + ")";
			}

			text.text += "\n";
		}
		if (detectedInteractable != null)
		{
			if (detectedInteractable.TryGetComponent(out IInteractMessage msgComponent))
			{
				text.text += msgComponent.GetInteractMessage();
			}
		}
		else if (detectedInteractable == null && detectedPickuppable == null)
		{
			text.text = null;
		}
    }
}
