using UnityEngine;

public class InteriorSceneManager : MonoBehaviour {
	private void Start () {
		InitialSceneLoader.OnInitialScenesLoaded += OnScenesLoaded;
		ScenePortalLibrary.BuildLibrary ();
	}

	private void OnScenesLoaded () {
		ScenePortalLibrary.BuildLibrary ();
	}
}
