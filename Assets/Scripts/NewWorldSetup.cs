using System.Collections.Immutable;
using IntroSequences;
using Items;
using UnityEngine;

/// Contains methods for setting up a new world after it is loaded for the first time.
public static class NewWorldSetup
{
	public static void PerformSetup(GameObject cameraRigPrefab)
	{
		// Handle the newly created player
		ActorData playerData = SaveInfo.NewlyCreatedPlayer;
		if (playerData == null)
		{
			// Kick back to the main menu
			Debug.LogError("No player data found for new world!");
			SceneChangeActivator.GoToMainMenu();
			return;
		}

		ActorRegistry.Register(playerData);

		// Run the intro sequence
		IIntroSequence introSeq = SaveInfo.IntroSequence;
		if (introSeq == null)
		{
			// Kick back to the main menu
			Debug.LogError("No intro sequence found for new world!");
			SceneChangeActivator.GoToMainMenu();
			return;
		}

		introSeq.Run(cameraRigPrefab, playerData.ActorId);
	}
}
