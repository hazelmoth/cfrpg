using System.Collections.Generic;
using System.Linq;
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
            Vector2 localPos = TilemapInterface.WorldPosToScenePos(transform.position, Scene);
            Vector2Int tilePos = TilemapInterface.FloorToTilePos(localPos);
            return new TileLocation(tilePos, Scene);
        }
    }
    
    public string Scene => SceneObjectManager.GetSceneIdForObject(gameObject);

    public EntityData GetData()
    {
        return ContentLibrary.Instance.Entities.Get(EntityId);
    }

    public List<SavedComponentState> GetSaveData()
    {
        return GetComponentData();
    }

    public void SetState(List<SavedComponentState> saved)
    {
        saveableComponents ??= GetComponents<ISaveable>();

        foreach (ISaveable component in saveableComponents)
        foreach (SavedComponentState savedComponent in saved.Where(
            savedComponent => component.ComponentId == savedComponent.componentId))
            component.SetTags(savedComponent.tags);
    }

    private List<SavedComponentState> GetComponentData()
    {
        saveableComponents ??= GetComponents<ISaveable>();
        return saveableComponents.Select(
                component => new SavedComponentState(
                    component.ComponentId,
                    component.GetTags().ToDictionary(entry => entry.Key, entry => entry.Value)))
            .ToList();
    }
}
