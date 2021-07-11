using System;
using System.Collections;
using UnityEngine;

/// A singleton MonoBehaviour to allow running coroutines and delayed functions
/// from non-MonoBehaviours.
public class GlobalCoroutineObject : MonoBehaviour
{
	private static GameObject instanceObject;
	private static GlobalCoroutineObject instance;
	public static MonoBehaviour Instance
	{
		get
		{
			if (instanceObject == null)
			{
				instanceObject = new GameObject("GlobalCoroutineObject");
				instance = instanceObject.AddComponent<GlobalCoroutineObject>();
			}
			return instance;
		}
	}

	/// Waits the provided number of seconds, then invokes the Action.
	/// Guaranteed to wait at least one frame.
	public static void InvokeAfter(float seconds, bool ignoreTimeScale, Action action)
	{
		Instance.StartCoroutine(InvokeAfter_Coroutine(seconds, ignoreTimeScale, action));
	}

	private static IEnumerator InvokeAfter_Coroutine(float seconds, bool ignoreTimeScale, Action action)
	{
		Func<float> currentTimeSupplier = () => ignoreTimeScale ? Time.unscaledTime : Time.time;
		float startTime = currentTimeSupplier.Invoke();
		// Always wait at least one frame
		yield return null;
		while (currentTimeSupplier.Invoke() - startTime < seconds) yield return null;
		action.Invoke();
	}
}
