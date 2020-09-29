using System.Collections.Generic;

[System.Serializable]
public struct SavedComponentState
{
	public string componentId;
	public IDictionary<string, string> tags;

	public SavedComponentState(string componentId, IDictionary<string, string> tags)
	{
		this.componentId = componentId;
		this.tags = tags;
	}
}
