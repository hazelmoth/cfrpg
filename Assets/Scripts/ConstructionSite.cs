using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : MonoBehaviour, ISaveable
{
    private const string SaveID = "TimedConstruction";
    private const string WorkSaveTag = "work";
    private const string TotalWorkSaveTag = "total_work";
    private const string GroundMaterialId = "construction";

    [SerializeField] private float totalWorkRequired;
    [SerializeField] private bool showGroundMarker = true;
    
    private GroundMaterial markerMaterial;
    private EntityData entity;
    private bool initialized;
    private string scene;

    public string EntityID { get; private set; }
    public float Work { get; private set; }

    string ISaveable.ComponentId => SaveID;


    // Start is called before the first frame update
    void Start()
    {
        if (!initialized)
            entity = new EntityData();
    }

    void Update()
    {
        //TEST 
        AddWork(Time.deltaTime);
    }

    public void AddWork (float amount)
    {
        Work += amount;
        if (Work >= totalWorkRequired)
        {
            Work = totalWorkRequired;
        }

        CheckIfFinished();
    }

    public void Initialize (string id)
    {
        EntityID = id;
        markerMaterial = ContentLibrary.Instance.GroundMaterials.Get(GroundMaterialId);
        scene = GetComponent<EntityObject>().Scene;
        entity = ContentLibrary.Instance.Entities.Get(id);
        totalWorkRequired = entity.workToBuild;
        Work = 0;
        initialized = true;
        PlaceGroundMarker();
        CheckIfFinished();

        if (string.IsNullOrEmpty(entity.entityId)) Debug.LogError("Initialized to nonexistent entity");

        if (markerMaterial == null) { Debug.LogWarning("Construction marker ground material not found!"); }
    }

    private void PlaceGroundMarker ()
    {
        if (!initialized) return;
        List<Vector2Int> baseShape = entity.baseShape;
        Vector2Int rootPos = TilemapInterface.WorldPosToScenePos(transform.position.ToVector2(), scene).ToVector2Int();
        foreach (Vector2Int pos in baseShape)
        {
            WorldMapManager.ChangeGroundMaterial(rootPos + pos, scene, TilemapLayer.GroundCover, markerMaterial);
        }
    }

    private void RemoveGroundMarker ()
    {
        if (!initialized) return;
        List<Vector2Int> baseShape = entity.baseShape;
        Vector2Int rootPos = TilemapInterface.WorldPosToScenePos(transform.position.ToVector2(), scene).ToVector2Int();
        foreach (Vector2Int pos in baseShape)
        {
            WorldMapManager.ChangeGroundMaterial(rootPos + pos, scene, TilemapLayer.GroundCover, null);
        }
    }

    private void CheckIfFinished ()
    {
        if (!initialized) return;

        if (Work < totalWorkRequired)
        {
            PlaceGroundMarker();
        }
        else
        {
            RemoveGroundMarker();
            PlaceFinishedEntity();
        }
    }

    private void PlaceFinishedEntity()
    {
        if (!initialized) return;
        Vector2Int scenePos = TilemapInterface.WorldPosToScenePos(transform.position, scene).ToVector2Int();
        WorldMapManager.RemoveEntityAtPoint(scenePos, scene);
        foreach (Vector2Int pos in entity.baseShape)
        {
            WorldMapManager.ForceClearEntityDataAtPoint(scenePos + pos, scene);
        }
        bool success = WorldMapManager.AttemptPlaceEntityAtPoint(entity, scenePos, scene);
        if (!success) Debug.LogError("Construction finished but entity failed to place!", this);
    }


    IDictionary<string, string> ISaveable.GetTags()
    {
        Dictionary<string, string> tags = new Dictionary<string, string>();
        tags.Add(WorkSaveTag, Work.ToString());
        tags.Add(TotalWorkSaveTag, totalWorkRequired.ToString());
        return tags;
    }

    void ISaveable.SetTags(IDictionary<string, string> tags)
    {
        Work = 0;
        totalWorkRequired = 0;

        if (tags.TryGetValue(TotalWorkSaveTag, out string totalTag))
        {
            totalWorkRequired = Int32.Parse(totalTag);
        }
        if (tags.TryGetValue(WorkSaveTag, out string workTag))
        {
            int work = Int32.Parse(workTag);
            AddWork(work);
        }

        CheckIfFinished();
    }
}
