using ActorAnim;
using UnityEngine;

public class ActorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ActorPrefab = null;
    private static ActorSpawner instance;

    private void Start()
    {
        instance = this;
    }
    /// Spawns the actor with the given ID and registers the spawned object with the registry.
    public static Actor Spawn(string actorId, Vector2 location, string scene, Direction direction = Direction.Down)
    {
	    if (instance == null)
	    {
		    Debug.LogError($"No instance of {typeof(ActorSpawner).FullName} found!");
		    return null;
	    };
	    if (!ActorRegistry.IdIsRegistered(actorId))
	    {
		    Debug.LogError($"Can't spawn actor \"{actorId}\"; ID not in registry!");
		    return null;
	    }
	    
        GameObject actorObject = GameObject.Instantiate(
            instance.ActorPrefab,
			TilemapInterface.ScenePosToWorldPos(location, scene),
            Quaternion.identity,
			SceneObjectManager.GetSceneObjectFromId(scene).transform
        );
		Actor actor = actorObject.GetComponent<Actor>();
		actor.Initialize(actorId);
        actor.MoveActorToScene(scene);
		actor.GetComponent<ActorSpriteController>().ForceDirection(direction);
        ActorRegistry.RegisterActorGameObject(actor);
		return actor;
    }
}
