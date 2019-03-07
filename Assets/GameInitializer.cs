using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the whole launch sequence (scene loading, asset loading, etc)
public class GameInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		// TEST obviously temporary
		PlayerDucats.SetDucatBalance (0);

		NotificationManager.Notify ("We're go.");
		EntityLibrary.LoadLibrary ();
		foreach(string id in EntityLibrary.GetEntityIdList()) {
			Debug.Log (id + ": " + EntityLibrary.GetEntityFromID (id));
		}

		// Load any mod assets
    }
}
