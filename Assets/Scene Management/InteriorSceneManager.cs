using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorSceneManager : MonoBehaviour {

	void Start () {
		SceneLoader.OnScenesLoaded += OnScenesLoaded;
		ScenePortalLibrary.BuildLibrary ();
	}
	void OnScenesLoaded () {
		ScenePortalLibrary.BuildLibrary ();
	}
}
