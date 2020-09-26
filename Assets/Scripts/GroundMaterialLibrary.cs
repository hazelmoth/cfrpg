using System.Collections.Generic;
using UnityEngine;

public class GroundMaterialLibrary
{
	private GroundMaterialLibraryObject libraryObject;
	private List<GroundMaterial> library;
	private const string MaterialLibraryPath = "GroundMaterialLibrary";

	private bool hasLoaded = false;


	// Must be called to use this class
	public void LoadLibrary () {
		GroundMaterialLibraryObject loadedLibraryAsset = (GroundMaterialLibraryObject)(Resources.Load (MaterialLibraryPath, typeof(ScriptableObject)));
		if (loadedLibraryAsset == null)
			Debug.LogError ("Ground material library not found!");

		libraryObject = loadedLibraryAsset;
		library = libraryObject.materials;

		hasLoaded = true;
	}

	public GroundMaterial Get (string id) {
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
