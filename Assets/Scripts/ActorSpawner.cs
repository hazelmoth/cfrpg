using UnityEngine;

public class ActorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ActorPrefab = null;
    private static ActorSpawner instance;

    private void Start()
    {
        instance = this;
    }
    // Spawns the actor with the given ID (assuming it's in the registry) and registers
    // the spawned object with the registry
	public static Actor Spawn(string ActorId, Vector2 location, string scene)
	{
		return Spawn(ActorId, location, scene, Direction.Down);
	}
	public static Actor Spawn(string ActorId, Vector2 location, string scene, Direction direction)
    {
	    if (instance == null)
	    {
		    Debug.LogError($"No instance of {typeof(ActorSpawner).FullName} found!");
		    return null;
	    };
	    
        GameObject ActorObject = GameObject.Instantiate(
            instance.ActorPrefab, 
			TilemapInterface.ScenePosToWorldPos(location, scene), 
            Quaternion.identity, 
			SceneObjectManager.GetSceneObjectFromId(scene).transform
        );
		Actor actor = ActorObject.GetComponent<Actor>();
		actor.Initialize(ActorId);
        actor.MoveActorToScene(SceneObjectManager.WorldSceneId);
		actor.GetComponent<ActorAnimController>().SetDirection(direction);
        ActorRegistry.RegisterActorGameObject(actor);
		return actor;
    }
}