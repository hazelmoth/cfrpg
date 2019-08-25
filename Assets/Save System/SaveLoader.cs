using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
	public delegate void SaveLoaderCallback();
	public delegate void SaveLoadedEvent();
	public static SaveLoadedEvent OnSaveLoaded;

	public static void LoadSave(WorldSave save, SaveLoaderCallback callback)
    {
		IEnumerator coroutine = LoadSaveCoroutine(save, callback);
		GlobalCoroutineObject.Instance.StartCoroutine(coroutine);
    }
	static IEnumerator LoadSaveCoroutine(WorldSave save, SaveLoaderCallback callback)
	{
		GameDataMaster.WorldName = save.worldName;
		WorldMapManager.LoadMap(save.worldMap.ToNonSerializable());
		OnSaveLoaded?.Invoke();
		callback?.Invoke();
		yield return null;
	}
}
