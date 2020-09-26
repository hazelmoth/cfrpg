
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
