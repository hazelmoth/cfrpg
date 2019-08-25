using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GroundMaterialLibrary
{
	static GroundMaterialLibraryObject libraryObject;
	static List<GroundMaterial> library;
	const string MaterialLibraryPath = "GroundMaterialLibrary";

	static bool hasLoaded = false;


	// Must be called to use this class
	public static void LoadLibrary () {
		GroundMaterialLibraryObject loadedLibraryAsset = (GroundMaterialLibraryObject)(Resources.Load (MaterialLibraryPath, typeof(ScriptableObject)));
		if (loadedLibraryAsset == null)
			Debug.LogError ("Ground material library not found!");

		libraryObject = loadedLibraryAsset;
		library = libraryObject.materials;

		hasLoaded = true;
	}

	public static GroundMaterial GetGroundMaterialById (string id) {
		if (!hasLoaded)
			LoadLibrary();
		foreach (GroundMaterial ground in library) {
			if (ground.materialId == id) {
				return ground;
			}
		}
		return null;
	}
}
