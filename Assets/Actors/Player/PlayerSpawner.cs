using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
	[SerializeField] private GameObject playerPrefab;


	private static PlayerSpawner instance;
    // Start is called before the first frame update
    private void Start()
    {
		instance = this;
		SceneChangeActivator.OnSceneExit += ResetEvents;
    }

    private void ResetEvents()
	{

	}

	public static void Spawn (PlayerCharData player, string scene, Vector2 location)
	{
		GameObject playerObject = GameObject.Instantiate(
			instance.playerPrefab,
			TilemapInterface.ScenePosToWorldPos(location, scene),
			Quaternion.identity,
			SceneObjectManager.GetSceneObjectFromId(scene).transform
		);
		
		if (player != null)
		{
			ActorRegistry.RegisterActor(
				new ActorData(player.saveId,
					player.playerName,
					null,
					null,
					Gender.Male,
					null,
					null,
					new ActorInventory.InvContents(),
					new FactionStatus(null)),
				playerObject.GetComponent<Player>());

			playerObject.GetComponent<Player>().Init(player.saveId);

			Debug.Log("loading player character");
			playerObject.GetComponent<Player>().GetData().Inventory.SetInventory(player.inventory.ToNonSerializable());
			playerObject.GetComponent<Player>().SetHair(player.hairId);
			playerObject.GetComponent<Player>().GetData().Personality = "western";
			playerObject.GetComponent<Player>().GetData().Race = player.raceId;
			// TEST
			playerObject.GetComponent<Player>().GetData().Inventory.AttemptAddItemToInv(ContentLibrary.Instance.Items.Get("axe"));
			PlayerController.SetPlayerActor(player.saveId);
		} 
		else {
			Debug.LogError("No player character loaded!");
		}

		Debug.Log("Playing as " + ActorRegistry.Get(player.saveId).data.ActorName);

	}
}
