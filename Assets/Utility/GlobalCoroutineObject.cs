using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCoroutineObject : MonoBehaviour
{
	static GameObject instanceObject;
	static GlobalCoroutineObject instance;
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
