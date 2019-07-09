using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
	[SerializeField] GameObject playerPrefab;
	public delegate void PlayerSpawnEvent();
	public static event PlayerSpawnEvent OnPlayerSpawned;

	static PlayerSpawner instance;
    // Start is called before the first frame update
    void Start()
    {
		instance = this;
		SceneChangeManager.OnSceneExit += ResetEvents;
    }
	void ResetEvents()
	{
		OnPlayerSpawned = null;
	}

	public static void Spawn (string scene, Vector2 location)
	{
		GameObject playerObject = GameObject.Instantiate(
			instance.playerPrefab,
			TilemapInterface.ScenePosToWorldPos(location, scene),
			Quaternion.identity,
			SceneObjectManager.GetSceneObjectFromId(scene).transform
		);
		if (GameDataMaster.LoadedPlayerChar != null)
		{
			Debug.Log("loading player character");
			playerObject.GetComponent<ActorInventory>().SetInventory(GameDataMaster.LoadedPlayerChar.inventory);
			playerObject.GetComponent<Player>().SetHair(GameDataMaster.LoadedPlayerChar.hairId);
		}
		Player.SetInstance(playerObject.GetComponent<Player>());
		OnPlayerSpawned?.Invoke();
	}
}
