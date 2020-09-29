using System.Collections.Generic;

// Describes an object whose state can be saved and loaded via tags
public interface ISaveable
{
	string ComponentId { get; }

	IDictionary<string, string> GetTags();

	void SetTags(IDictionary<string, string> tags);
}