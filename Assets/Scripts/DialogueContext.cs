/// Describes who is the speaker and who is the target at a particular point in conversation.
public class DialogueContext
{
	public readonly string speakerActorId;
	public readonly string targetActorId;

	public DialogueContext(string speakerActorId, string targetActorId)
	{
		this.speakerActorId = speakerActorId;
		this.targetActorId = targetActorId;
	}
}
