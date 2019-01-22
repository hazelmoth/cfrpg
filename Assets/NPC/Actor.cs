using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// A parent class to encompass both the player and NPCs, for the purpose of things like NPC pathfinding
// and teleporting actors between scenes when they activate portals.
public abstract class Actor : MonoBehaviour
{
	protected string actorCurrentScene = "World";
	public void MoveActorToScene (string scene) {

		actorCurrentScene = scene;
		this.gameObject.transform.SetParent(SceneManager.GetSceneByName (scene).GetRootGameObjects()[0].transform);

	}
}

