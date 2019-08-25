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
		SceneChangeActivator.OnSceneExit += ResetEvents;
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
		playerObject.GetComponent<Player>().Init();
		if (GameDataMaster.LoadedPlayerChar != null)
		{
			Debug.Log("loading player character");
			playerObject.GetComponent<Player>().Inventory.SetInventory(GameDataMaster.LoadedPlayerChar.inventory);
			playerObject.GetComponent<Player>().SetHair(GameDataMaster.LoadedPlayerChar.hairId);
			// TEST
			playerObject.GetComponent<Player>().Inventory.AttemptAddItemToInv(ItemManager.GetItemById("cowboy_hat"));
		} 
		else {
			Debug.LogError("No player character loaded!");
		}
		Player.SetInstance(playerObject.GetComponent<Player>());
		OnPlayerSpawned?.Invoke();
	}
}
