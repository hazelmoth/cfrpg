using JetBrains.Annotations;
using Popcron.Console;
using UnityEngine;

namespace Dialogue
{
    /// A set of static methods which are accessible in dialogue scripts as commands.
    [UsedImplicitly]
    public static class DialogueCommands
    {
        /// Subtracts the given amount from both the player's debt and balance.
        [Command("pay_debt")]
        public static void PayPlayerDebt(int amount)
        {
            ActorData playerData = PlayerController.GetPlayerActor()?.GetData();
            if (playerData == null)
            {
                Debug.LogError("Failed to retrieve player data.");
                return;
            }

            ActorWallet wallet = playerData.Get<ActorWallet>();
            if (wallet == null) return;
            
            wallet.AddBalance(-amount);
            wallet.Debt -= amount;
        }
    }
}
