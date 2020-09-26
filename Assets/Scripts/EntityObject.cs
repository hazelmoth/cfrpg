using System.Collections.Generic;
using UnityEngine;

public class EntityObject : MonoBehaviour
{
	[SerializeField] public string entityId;
	[SerializeField] private List<SaveableComponent> saveableComponents;

	public string Scene
	{
		get { return SceneObjectManager.GetSceneIdForObject(gameObject); }
	}
	public SavedEntity GetStateData ()
	{
        return new SavedEntity(entityId, Scene, TilemapInterface.WorldPosToScenePos(transform.position, Scene).ToVector2Int(), GetComponentData());
    }
    public void SetStateData(SavedEntity saved)
    {
        if (saved.id != entityId)
        {
            Debug.LogError("this entity doesn't seem to match the provided data");
        }
		if (saveableComponents != null)
		{
			foreach (SaveableComponent component in saveableComponents)
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
    }
	protected List<SavedComponentState> GetComponentData ()
	{
		List<SavedComponentState> savedComponents = new List<SavedComponentState>();
		if (saveableComponents != null)
		{
			foreach (SaveableComponent component in saveableComponents)
			{
				savedComponents.Add(component.GetSaveState());
			}
		}
		return savedComponents;
	}
}
