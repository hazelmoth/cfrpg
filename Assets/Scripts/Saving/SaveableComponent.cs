using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveableComponent : MonoBehaviour
{
	public abstract string ComponentId { get; }
	public abstract List<string> Tags { get; }

	public abstract void SetTags(List<string> tags);

	public SavedComponentState GetSaveState ()
	{
		return new SavedComponentState(ComponentId, Tags);
	}
}
