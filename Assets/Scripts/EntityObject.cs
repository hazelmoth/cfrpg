using System.Collections.Generic;
using UnityEngine;

public class EntityObject : MonoBehaviour
{
	private ISaveable[] saveableComponents;

	public string EntityId { get; set; }

	public string Scene
	{
		get { return SceneObjectManager.GetSceneIdForObject(gameObject); }
	}
	public SavedEntity GetSaveData ()
	{
        return new SavedEntity(EntityId, Scene, TilemapInterface.WorldPosToScenePos(transform.position, Scene).ToVector2Int(), GetComponentData());
    }
    public void SetStateData(SavedEntity saved)
    {
        if (saved.id != EntityId)
        {
            Debug.LogError("This entity doesn't seem to match the provided data");
        }
		if (saveableComponents == null)
		{
			saveableComponents = GetComponents<ISaveable>();
		}
		foreach (ISaveable component in saveableComponents)
		{
			foreach (SavedComponentState savedComponent in saved.components)
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
