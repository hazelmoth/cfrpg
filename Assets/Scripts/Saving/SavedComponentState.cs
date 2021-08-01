using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

[System.Serializable]
public struct SavedComponentState
{
	public string componentId;
	public Dictionary<string, string> tags;

	public SavedComponentState(string componentId, Dictionary<string, string> tags)
	{
		this.componentId = componentId;
		this.tags = tags;
	}

	public override string ToString()
	{
		return componentId + ": " +
		       string.Join(", ", tags.Select(entry => entry.Key + " = " + entry.Value));
	}
}
