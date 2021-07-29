using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Builds a ScriptableObject in the editor to store entity data
namespace ContentLibraries
{
    public static class GenericLibraryBuilder
    {
        public static void Build<T, TLibrary>(string contentFolderPath, string libraryPath)
            where T : ScriptableObject, IContentItem
            where TLibrary : LibraryAsset<T>
        {
            List<T> contentItems = ReadContent<T>(contentFolderPath);

            // Create a new library prefab
            TLibrary libraryObject = ScriptableObject.CreateInstance<TLibrary>();
            AssetDatabase.CreateAsset(libraryObject, "Assets/" + libraryPath);

            // Relocate the created prefab in the assets folder
            TLibrary loadedLibraryAsset =
                (TLibrary) AssetDatabase.LoadAssetAtPath("Assets/" + libraryPath, typeof(ScriptableObject));
            
            // Make some persistent changes
            Undo.RecordObject(loadedLibraryAsset, "Build content library prefab");
            loadedLibraryAsset.content = contentItems;
            PrefabUtility.RecordPrefabInstancePropertyModifications(loadedLibraryAsset);
            EditorUtility.SetDirty(loadedLibraryAsset);


            // Double check that that worked
            if (loadedLibraryAsset == null || loadedLibraryAsset.content == null)
                Debug.LogWarning(typeof(T).Name + " library build failed!");
            else
                Debug.Log(typeof(T).Name + " library built.");
        }

        private static List<T> ReadContent<T>(string contentFolderPath) where T : ScriptableObject
        {
            List<T> results = new List<T>();
            DirectoryInfo contentFolder = new DirectoryInfo(Path.Combine(Application.dataPath, contentFolderPath));

            foreach (FileInfo asset in contentFolder.EnumerateFiles("*.asset", SearchOption.AllDirectories))
            {
                string dataObjectPath = "Assets/" + contentFolderPath + "/" +
                                        (asset.Directory!.Name.Equals(contentFolder.Name) ? "" : asset.Directory!.Name + "/") +
                                        asset.Name;
                T dataObject = (T) AssetDatabase.LoadMainAssetAtPath(dataObjectPath);
                if (dataObject != null)
                    results.Add(dataObject);
                else
                    Debug.LogWarning("Failed to cast content object from \"" + dataObjectPath + "\"!");
            }

            return results;
        }
    }
}