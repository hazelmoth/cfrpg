// Stores what faction an actor is in (if any) and their
// role/status in that faction

using System;

namespace ActorComponents
{
	[Serializable]
	public class FactionStatus : IActorComponent
	{
		public FactionStatus(string factionId)
		{
			FactionId = factionId;
		}

		public string FactionId { get; set; }

		public string AssignedJob { get; set; }

		// Null for 'don't accompany'
		public string AccompanyTarget { get; set; }
	}
}
