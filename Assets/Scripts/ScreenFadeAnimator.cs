using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeAnimator : MonoBehaviour
{
	[SerializeField]
	private Image fadePanel;

	private static ScreenFadeAnimator instance;

	private Coroutine activeFade;

	private void Start()
	{
		instance = this;
		fadePanel.canvasRenderer.SetAlpha(0f);
	}

	public static void FadeOut(float duration)
	{
		if (instance == null)
		{
			Debug.LogError("Can't do screen fade; no instance available.");
		}
		instance.fadePanel.CrossFadeAlpha(1f, duration, true);
	}

	public static void FadeIn(float duration)
	{
		if (instance == null)
		{
			Debug.LogError("Can't do screen fade; no instance available.");
		}
		instance.fadePanel.CrossFadeAlpha(0f, duration, true);
	}
}
