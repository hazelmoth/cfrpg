namespace Dialogue
{
	/// Describes who is the player and who is the conversant in a particular conversation.
	public class DialogueContext
	{
		public readonly string playerId;
		public readonly string nonPlayerId;

		public DialogueContext(string playerId, string nonPlayerId)
		{
			this.playerId = playerId;
			this.nonPlayerId = nonPlayerId;
		}

		public static DialogueContext WithPlayer(string conversantActorId)
		{
			return new DialogueContext(PlayerController.PlayerActorId, conversantActorId);
		}
	}
}
