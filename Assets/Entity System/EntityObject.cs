using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityObject : MonoBehaviour
{
	[SerializeField] public string entityId;

	public virtual void SetTag(string tag, string value)
	{
		if (tag == "id")
		{
			entityId = value;
		}
		else
		{
			Debug.LogWarning("tag not found");
		}
	}
	public EntityState GetStateData ()
	{
		return new EntityState(entityId, GetTagData());
	}
	protected virtual List<string> GetTagData ()
	{
		return new List<string> { entityId };
	}
}
