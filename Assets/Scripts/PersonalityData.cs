using ContentLibraries;
using UnityEngine;

[CreateAssetMenu(fileName = "data", menuName = "Personality Type Asset")]
public class PersonalityData : ScriptableObject, IContentItem
{
	[SerializeField] private string id;
	[SerializeField] private float frequency = 1f;
	[SerializeField] private TextAsset dialogueAsset;

	private DialoguePack dialoguePack;
	public string Id => id;
	public float Frequency => frequency;

	public DialoguePack GetDialoguePack()
	{
		if (dialoguePack == null)
		{
			dialoguePack = new DialoguePack(dialogueAsset);
		}

		return dialoguePack;
	}
}
