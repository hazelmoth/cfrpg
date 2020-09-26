using System.Collections.Generic;

[System.Serializable]
public struct SavedComponentState
{
	public string componentId;
	public List<string> tags;

	public SavedComponentState(string componentId, List<string> tags)
	{
		this.componentId = componentId;
		this.tags = tags;
	}
}
