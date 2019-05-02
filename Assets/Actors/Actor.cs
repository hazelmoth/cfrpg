using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// A parent class to encompass both the player and NPCs, for the purpose of things like NPC pathfinding
// and teleporting actors between scenes when they activate portals.
public abstract class Actor : MonoBehaviour
{
	protected string actorCurrentScene = SceneObjectManager.WorldSceneId;
	public string ActorCurrentScene {get{return actorCurrentScene;}}
	public Direction Direction { get { return GetComponent<HumanAnimController>().GetDirection(); }}
	protected NPCBehaviourAI behaviourAi;
	protected ActorPhysicalCondition physicalCondition;
	protected ActorInventory inventory;


	public NPCBehaviourAI BehaviourAI {
		get {
			if (behaviourAi == null) {
				return GetComponent<NPCBehaviourAI> ();
			} else {
				return behaviourAi;
			}
		}
	}
	public ActorPhysicalCondition PhysicalCondition {
		get {
			if (physicalCondition == null) {
				return GetComponent<ActorPhysicalCondition> ();
			} else {
				return physicalCondition;
			}
		}
	}
	public ActorInventory Inventory {
		get {
			if (inventory == null) {
				return GetComponent<ActorInventory> ();
			} else {
				return inventory;
			}
		}
	}


	public void MoveActorToScene (string scene) {

		actorCurrentScene = scene;
		GameObject sceneRoot = SceneObjectManager.GetSceneObjectFromId (scene);
		if (sceneRoot != null) {
			this.gameObject.transform.SetParent (sceneRoot.transform);
		}
	}
}

