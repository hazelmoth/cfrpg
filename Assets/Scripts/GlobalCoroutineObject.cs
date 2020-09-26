using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			return instance as MonoBehaviour;
		}
	}
}
