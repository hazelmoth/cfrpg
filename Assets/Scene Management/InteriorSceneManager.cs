using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorSceneManager : MonoBehaviour {

	void Start () {
		SceneLoader.OnScenesLoaded += OnScenesLoaded;
	}
	void OnScenesLoaded () {
		ScenePortalLibrary.BuildLibrary ();
	}
}
