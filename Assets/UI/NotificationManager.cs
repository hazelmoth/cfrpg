using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationManager : MonoBehaviour
{

	[SerializeField] Image notificationPanel;
	[SerializeField] TextMeshProUGUI notificationText;

	const float NotificationLifetime = 1.5f;
	const float FadeTime = 0.8f;

	static NotificationManager instance;
	static Color visiblePanelColor;
	static Color visibleTextColor;

    // Start is called before the first frame update
    void Start()
    {
		instance = this;
		visiblePanelColor = notificationPanel.color;
		visibleTextColor = notificationText.color;
		SetVisible (false);
    }

	public static void Notify (string text) {
		instance.StopAllCoroutines ();
		SetText (text);
		SetVisible (true);
		instance.StartCoroutine (WaitAndFade());
	}
	static void SetVisible (bool visible) {
		if (visible) {
			instance.notificationPanel.color = visiblePanelColor;
			instance.notificationText.color = visibleTextColor;
		} else {
			instance.notificationPanel.color = Color.clear;
			instance.notificationText.color = Color.clear;
		}
	}
	static void SetText (string text) {
		instance.notificationText.text = text;
	}
	static IEnumerator WaitAndFade () {
		float start = Time.time;
		while (Time.time - start < NotificationLifetime)
			yield return null;
		instance.notificationPanel.CrossFadeAlpha (0f, FadeTime, true);
		instance.notificationText.CrossFadeAlpha (0f, FadeTime, true);
	}
}
