using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor {

	public static Player instance;

	// Use this for initialization
	void Start () {
		instance = this;
		actorCurrentScene = this.gameObject.scene.name;
	}
}
