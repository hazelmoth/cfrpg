using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the whole launch sequence (scene loading, asset loading, etc)
public class GameInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		EntityLibrary.LoadLibrary ();
		GroundMaterialLibrary.LoadLibrary ();

		// Load any mod assets

		// TEST
		WorldMapManager.LoadMap(WorldMapGenerator.Generate(50, 50));
		WorldMapManager.LoadMapsIntoScenes();
		Debug.Log(WorldMapManager.AttemptPlaceEntityAtPoint(EntityLibrary.GetEntityFromID("tent_green"), new Vector2Int(0, 0), "World"));

		// TEST obviously temporary
		PlayerDucats.SetDucatBalance (0);


		NotificationManager.Notify ("We're go.");
    }
}
