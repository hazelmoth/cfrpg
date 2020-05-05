using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairLibrary
{
	private const string LIBRARY_ASSET_PATH = "HairLibrary";

    private List<Hair> hairs;

    public void LoadLibrary ()
	{
		HairLibraryAsset loadedLibraryAsset = (HairLibraryAsset)(Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));

		hairs = loadedLibraryAsset.hairs;
    }

    public List<Hair> GetHairs ()
    {
		return hairs;
    }
    public Hair GetById(string id)
    {
		if (id == string.Empty)
		{
			return null;
		}
        foreach (Hair hair in hairs)
        {
            if (hair.hairId == id)
                return hair;
        }
        Debug.Log("Hair ID \"" + id + "\" not found.");
        return null;
    }
}
