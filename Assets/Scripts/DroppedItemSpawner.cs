using UnityEngine;

public class DroppedItemSpawner : MonoBehaviour
{
    private static DroppedItemSpawner instance;
    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private DroppedItemRegistry registry;

    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
    }

    public static DroppedItem SpawnItem(ItemStack item, Vector2 position, string scene, bool randomlyShiftPosition)
    {
        if (randomlyShiftPosition)
            position = new Vector2(position.x + Random.Range(-0.5f, 0.5f), position.y + Random.Range(-0.5f, 0.5f));
        return SpawnItem(item, position, scene);
    }

    public static DroppedItem SpawnItem(ItemStack item, Vector2 position, string scene)
    {
        // make sure we've been initialized
        if (instance == null)
        {
            instance = FindObjectOfType<DroppedItemSpawner>();

            if (instance == null)
                return null;
        }

        // Check if this is the ID of an itemized actor, and if so spawn that actor instead
        if (item.Id != null
            && ItemIdParser.ParseBaseId(item.Id) == "corpse"
            && item.GetModifiers().ContainsKey("actor_id"))
        {
            string actorId = item.GetModifiers()["actor_id"];
            ActorSpawner.Spawn(actorId, position, scene);
            return null;
        }

        GameObject newItem = Instantiate(instance.droppedItemPrefab);
        DroppedItem droppedItem = newItem.GetComponent<DroppedItem>();
        droppedItem.SetItem(item);
        droppedItem.Scene = scene;

        newItem.transform.SetParent(SceneObjectManager.GetSceneObjectFromId(scene).transform);
        newItem.transform.localPosition = position;

        if (instance.registry == null)
            Debug.LogError("Missing reference to dropped item registry!");
        else
            instance.registry.RegisterItem(droppedItem);

        return droppedItem;
    }
}
 
