using UnityEngine;

public class PauseManager : MonoBehaviour
{
	private static bool paused = false;
	public static bool Paused {get {return paused;}}
	private static float originalTimeScale = 1.0f;

	public static void Pause () {
		if (paused)
			return;
		originalTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		paused = true;
	}
	public static void Unpause () {
		Time.timeScale = originalTimeScale;
		paused = false;
	}
}
