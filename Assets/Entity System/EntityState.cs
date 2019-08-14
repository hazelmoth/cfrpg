using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EntityState
{
	public string id;
	public List<string> tags;

	public EntityState(string id, List<string> tags)
	{
		this.id = id;
		this.tags = tags;
	}
}
