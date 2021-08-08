using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using TMPro;
using UnityEngine;

public class ConstructionSite : MonoBehaviour, ISaveable, IContinuouslyInteractable, IInteractMessage
{
    private const string SaveID = "TimedConstruction";
    private const string WorkSaveTag = "work";
    private const string TotalWorkSaveTag = "total_work";
    private const string EntitySaveTag = "entity";
    private const string GroundMaterialId = "construction";
    private const string InteractMessage = "Hold E to construct";
    private const string CurrentlyInteractingMessage = "Constructing...";

    [SerializeField] private float totalWorkRequired;
    [SerializeField] private bool showGroundMarker = true;
    [SerializeField] private GameObject progressDisplayPrefab = null;

    private GroundMaterial markerMaterial;
    private EntityData entity;
    private TextMeshProUGUI progressDisplayText;
    private bool initialized;
    private string scene;
    private bool interactedThisFrame;
    private bool interactedLastFrame;

    public string EntityID { get; private set; }
    public float Work { get; private set; }
    public float TotalWorkRequired => totalWorkRequired;
    public bool ConstructionFinished => Work < TotalWorkRequired;

    string ISaveable.ComponentId => SaveID;

    void Update()
    {
        if (initialized) UpdateProgressDisplay();
    }

    void LateUpdate()
    {
        interactedLastFrame = interactedThisFrame;
        interactedThisFrame = false;
    }

    public void AddWork(float amount)
    {
        Work += amount;
        if (Work >= totalWorkRequired)
        {
            Work = totalWorkRequired;
        }

        CheckIfFinished();
    }

    public void Initialize(string id)
    {
        Debug.Assert(progressDisplayPrefab != null, "Progress Display prefab hasn't been assigned in the inspector!");

        EntityID = id;
        markerMaterial = ContentLibrary.Instance.GroundMaterials.Get(GroundMaterialId);
        scene = GetComponent<EntityObject>().Scene;
        entity = ContentLibrary.Instance.Entities.Get(id);

        Debug.Assert(entity != null, "Construction site initialized with nonexistent entity ID!");

        totalWorkRequired = entity.WorkToBuild;
        Work = 0;
        initialized = true;

        if (showGroundMarker)
        {
            PlaceGroundMarker();
        }
        CreateProgressDisplay();

        CheckIfFinished();

        Debug.Assert(markerMaterial != null, "Construction marker ground material not found!");
        Debug.Assert(!string.IsNullOrEmpty(entity.Id), "Initialized to nonexistent entity!");
    }

    private void CreateProgressDisplay()
    {
        Debug.Assert(progressDisplayText == null, "Display text already rendering!");

        Vector2 location = GetCenter(entity.BaseShape) + new Vector2(0.5f, 0.5f);
        GameObject createdObject = GameObject.Instantiate(progressDisplayPrefab, transform);
        createdObject.transform.localPosition = location;

        progressDisplayText = createdObject.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Assert(progressDisplayText != null, "Prefab missing progressDisplayText component!");
        UpdateProgressDisplay();
    }

    private void UpdateProgressDisplay ()
    {
        Debug.Assert(progressDisplayText != null);
        progressDisplayText.text = Mathf.FloorToInt(Work) + "/" + Mathf.FloorToInt(TotalWorkRequired);
    }

    private void PlaceGroundMarker()
    {
        Debug.Assert(initialized);

        IEnumerable<Vector2Int> baseShape = entity.BaseShape;
        Vector2Int rootPos = TilemapInterface.WorldPosToScenePos(transform.position.ToVector2(), scene).ToVector2Int();
        foreach (Vector2Int pos in baseShape)
        {
            RegionMapManager.ChangeGroundMaterial(rootPos + pos, scene, TilemapLayer.GroundCover, markerMaterial);
        }
    }

    private void RemoveGroundMarker()
    {
        Debug.Assert(initialized);

        IEnumerable<Vector2Int> baseShape = entity.BaseShape;
        Vector2Int rootPos = TilemapInterface.WorldPosToScenePos(transform.position.ToVector2(), scene).ToVector2Int();
        foreach (Vector2Int pos in baseShape)
        {
            RegionMapManager.ChangeGroundMaterial(rootPos + pos, scene, TilemapLayer.GroundCover, null);
        }
    }

    private void CheckIfFinished()
    {
        if (!initialized) return;

        if (Work >= totalWorkRequired)
        {
            RemoveGroundMarker();
            PlaceFinishedEntity();
        }
    }

    private void PlaceFinishedEntity()
    {
        if (!initialized) return;
        Vector2Int scenePos = TilemapInterface.WorldPosToScenePos(transform.position, scene).ToVector2Int();
        RegionMapManager.RemoveEntityAtPoint(scenePos, scene);
        foreach (Vector2Int pos in entity.BaseShape)
        {
            RegionMapManager.ForceClearEntityDataAtPoint(scenePos + pos, scene);
        }
        bool success = RegionMapManager.AttemptPlaceEntityAtPoint(entity, scenePos, scene);
        if (!success) Debug.LogError("Construction finished but entity failed to place!", this);
    }


    IDictionary<string, string> ISaveable.GetTags()
    {
        Dictionary<string, string> tags = new Dictionary<string, string>();
        tags.Add(EntitySaveTag, EntityID);
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
            totalWorkRequired = float.Parse(totalTag);
        }
        if (tags.TryGetValue(WorkSaveTag, out string workTag))
        {
            float work = float.Parse(workTag);
            AddWork(work);
        }
        if (tags.TryGetValue(EntitySaveTag, out string value))
        {
            Initialize(value);
        }
        else
        {
            Debug.LogError("Construction site missing entity save tag!", this);
        }

        CheckIfFinished();
    }

    // Returns a point in the center of the bounding box created by the given points.
    private static Vector2 GetCenter(IReadOnlyCollection<Vector2Int> points) 
    {
        Debug.Assert(points != null, "Given list is null!");
        Debug.Assert(points.Count > 0, "Given list is empty!");

        var xVals =
            from point in points
            select point.x;
        var yVals =
            from point in points
            select point.y;

        float maxX = xVals.Max();
        float minX = xVals.Min();
        float maxY = yVals.Max();
        float minY = yVals.Min();

        return new Vector2((maxX + minX) / 2, (maxY + minY) / 2);
    }

    public void Interact()
    {
        AddWork(Time.deltaTime * 10);  // Adds 10 work per second.
        interactedThisFrame = true;
    }

    public string GetInteractMessage()
    {
        return interactedThisFrame || interactedLastFrame ? CurrentlyInteractingMessage : InteractMessage;
    }
}
