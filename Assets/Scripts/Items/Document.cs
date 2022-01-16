using System.Collections.Generic;
using GUI;
using UnityEngine;

namespace Items
{
    /// An item containing a message the player can read.
    [CreateAssetMenu(fileName = "new_item", menuName = "Items/Letter")]
    public class Document : ItemData, IActivatable
    {
        public const string SenderNameModifier = "sender";
        public const string MessageModifier = "message";

        /// Takes the item modifiers for a particular letter and returns the sender's name.
        public static string GetSender(IDictionary<string, string> modifiers)
        {
            return modifiers.ContainsKey(SenderNameModifier) ? modifiers[SenderNameModifier] : null;
        }

        /// Takes the item modifiers for a particular letter and returns the message.
        /// Returns an empty string if there is no message modifier.
        public static string GetMessage(IDictionary<string, string> modifiers)
        {
            string msg = modifiers.ContainsKey(MessageModifier) ? modifiers[MessageModifier] : "";

            return msg;
        }

        public void Activate(IDictionary<string, string> modifiers, Actor user)
        {
            if (PlayerController.PlayerActorId == null || user.ActorId != PlayerController.PlayerActorId) return;

            string message = GetMessage(modifiers);
            Debug.Assert(message != null, "Message is null");

            FindObjectOfType<DocumentUiManager>()?.SetMessage(message);
            UIManager.SwitchToDocumentCanvas();
        }
    }
}
