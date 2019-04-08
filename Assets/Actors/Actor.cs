using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// A parent class to encompass both the player and NPCs, for the purpose of things like NPC pathfinding
// and teleporting actors between scenes when they activate portals.
public abstract class Actor : MonoBehaviour
{
	protected string actorCurrentScene = SceneObjectManager.WorldSceneId;
	// Needs to be set by anything inheriting from this class
	protected ActorCondition actorCondition;

	public string ActorCurrentScene {get{return actorCurrentScene;}}
	public void MoveActorToScene (string scene) {

		actorCurrentScene = scene;
		GameObject sceneRoot = SceneObjectManager.GetSceneObjectFromId (scene);
		if (sceneRoot != null) {
			this.gameObject.transform.SetParent (sceneRoot.transform);
		}
	}
}

