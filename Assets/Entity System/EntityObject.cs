using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityObject : MonoBehaviour
{
	[SerializeField] public string entityId;
	[SerializeField] List<SaveableComponent> saveableComponents;

	public string Scene
	{
		get { return SceneObjectManager.GetSceneIdForObject(gameObject); }
	}
	public SavedEntity GetStateData ()
	{
		return new SavedEntity(entityId, Scene, TilemapInterface.WorldPosToScenePos(this.transform.position, Scene).ToVector2Int(), GetComponentData());
	}
	protected virtual List<SavedComponentState> GetComponentData ()
	{
		List<SavedComponentState> savedComponents = new List<SavedComponentState>();
		foreach (SaveableComponent component in saveableComponents)
		{
			savedComponents.Add(component.GetSaveState());
		}
		return savedComponents;
	}
}
