using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
	private static bool gameIsPaused = false;
	public static bool GameIsPaused {get {return gameIsPaused;}}
	private static float originalTimeScale = 1.0f;

	public static void Pause () {
		if (gameIsPaused)
			return;
		originalTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		gameIsPaused = true;
	}
	public static void Unpause () {
		Time.timeScale = originalTimeScale;
		gameIsPaused = false;
	}
}
