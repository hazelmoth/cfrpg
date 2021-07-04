using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

public class EntityObject : MonoBehaviour
{
	private ISaveable[] saveableComponents;

	public string EntityId { get; set; }

	public TileLocation Location
	{
		get
		{
			Vector2 localPos = TilemapInterface.WorldPosToScenePos(transform.position, SceneObjectManager.WorldSceneId);
			Vector2Int tilePos = TilemapInterface.FloorToTilePos(localPos);
			return new TileLocation(tilePos, Scene);
		}
	}


	public string Scene => SceneObjectManager.GetSceneIdForObject(gameObject);

	public EntityData GetData ()
	{
		return ContentLibrary.Instance.Entities.Get(EntityId);
	}

	public List<SavedComponentState> GetSaveData ()
	{
        return GetComponentData();
    }

    public void SetState(List<SavedComponentState> saved)
    {
	    if (saveableComponents == null)
		{
			saveableComponents = GetComponents<ISaveable>();
		}
		foreach (ISaveable component in saveableComponents)
		{
			foreach (SavedComponentState savedComponent in saved)
			{
				if (component.ComponentId == savedComponent.componentId)
				{
					component.SetTags(savedComponent.tags);
				}
			}
		}
    }

	private List<SavedComponentState> GetComponentData ()
	{
		List<SavedComponentState> savedComponents = new List<SavedComponentState>();
		if (saveableComponents == null)
		{
			saveableComponents = GetComponents<ISaveable>();
		}
		foreach (ISaveable component in saveableComponents)
		{
			savedComponents.Add(new SavedComponentState(component.ComponentId, component.GetTags()));
		}
		return savedComponents;
	}
}
