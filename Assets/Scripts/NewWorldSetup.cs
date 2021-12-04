using System.Collections.Immutable;
using UnityEngine;

/// Contains methods for setting up a new world after it is loaded for the first time
public static class NewWorldSetup
{
	private static readonly ImmutableList<ItemStack> StartingInv =
		ImmutableList.Create(new ItemStack("wheat_seeds", 12));
	
	public static void PerformSetup(GameObject cameraRigPrefab)
	{
		// Handle the newly created player
		ActorData playerData = SaveInfo.NewlyCreatedPlayer;
		if (playerData == null)
		{
			Debug.LogError("No player data found for new world!");
			// Kick back to the main menu
			SceneChangeActivator.GoToMainMenu();
		}
		else
		{
			// Set inventory
			StartingInv.ForEach(
				stack => playerData.Inventory.AttemptAddItem(stack));

			ActorRegistry.Register(playerData);

			// Run the intro sequence
			GameObject introSequenceObj = new("IntroSequenceScript");
			IntroSequenceScript introSequence =
				introSequenceObj.AddComponent<IntroSequenceScript>();

			introSequence.RunIntroSequence(cameraRigPrefab, playerData.ActorId);
		}
	}
}
