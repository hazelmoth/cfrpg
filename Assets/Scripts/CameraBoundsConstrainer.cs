using UnityEngine;

public class CameraBoundsConstrainer : MonoBehaviour
{
    private Camera cam;
    private Vector3 cameraOffsetFromRig;
    [SerializeField] private GameObject cameraParent;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        if (!cam)
        {
            Debug.LogError("Camera bounds constrainer is missing a camera component?");
        }
        cameraOffsetFromRig = cam.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!cam || PauseManager.GameIsPaused) return;

        string scene = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.CurrentScene;
        // Don't constrain the camera if we're not in the outdoor world scene
        if (scene != SceneObjectManager.WorldSceneId)
        {
            return;
        }

        GameObject sceneObject = SceneObjectManager.GetSceneObjectFromId(SceneObjectManager.WorldSceneId);
        Vector2 sceneBottomLeft = Vector2.zero + sceneObject.transform.position.ToVector2();
        Vector2 sceneTopRight = sceneBottomLeft + new Vector2(SaveInfo.RegionSize.x, SaveInfo.RegionSize.y);

        float camWidth = cam.ViewportToWorldPoint(Vector2.one).x - transform.position.x;
        float camHeight = cam.orthographicSize;

        Vector3 targetPos = cameraParent.transform.position;

        targetPos.x = Mathf.Max(transform.position.x, sceneBottomLeft.x + camWidth);
        targetPos.x = Mathf.Min(transform.position.x, sceneTopRight.x - camWidth);
        targetPos.y = Mathf.Max(transform.position.y, sceneBottomLeft.y + camHeight);
        targetPos.y = Mathf.Min(transform.position.y, sceneTopRight.y - camHeight);

        transform.position = targetPos + (Vector3)cameraOffsetFromRig;
    }
}
