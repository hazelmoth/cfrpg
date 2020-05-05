using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class FullscreenShortcut
{
	static FullscreenShortcut()
	{
		EditorApplication.update += Update;
	}

	private static void Update()
	{
#if UNITY_EDITOR
		if (EditorApplication.isPlaying && ShouldToggleMaximize())
		{
			EditorWindow.focusedWindow.maximized = !EditorWindow.focusedWindow.maximized;
		}
#endif
	}

	private static bool ShouldToggleMaximize()
	{
		return Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift);
	}
}