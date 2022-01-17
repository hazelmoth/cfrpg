using System.Collections.Immutable;
using Items;
using UnityEngine;

/// Contains methods for setting up a new world after it is loaded for the first time.
public static class NewWorldSetup
{
	private const string MortgageLetterText =
		"Her Majesty's Year 760.\n" +
		"\n" +
		"Dear Sir or Madam,\n" +
		"\n" +
		"I am pleased to inform you that your mortgage deed concerning 1.77 octopares of land in the Crescian " +
		"Central Valley has been completed. Your mortgage totals Eight Thousand, Eight Hundred Dollars to be " +
		"paid in the next year.\n" +
		"\n" +
		"Yours faithfully,\n" +
		"\n" +
		"E.J.\n" +
		"The Second Imperial Bank of Lyndon.";

	private static readonly ImmutableList<ItemStack> StartingInv =
		ImmutableList.Create(
			new ItemStack("wheat_seeds", 12),
			new ItemStack("letter", 1)
				.WithModifier(ItemData.ItemNameModifier, "Mortgage Letter")
				.WithModifier(Document.SenderNameModifier, "the Bank of Crescia")
				.WithModifier(Document.MessageModifier, MortgageLetterText));
	
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
