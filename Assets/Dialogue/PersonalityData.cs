using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "data", menuName = "Personality Type Asset")]
public class PersonalityData : ScriptableObject
{
	[SerializeField] private string personalityId;
	[SerializeField] private float frequency = 1f;
	[SerializeField] private TextAsset dialogueAsset;

	private DialoguePack dialoguePack;

	public string PersonalityId => personalityId;
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
