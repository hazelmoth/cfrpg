using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] GameObject npcPrefab = null;
    static NPCSpawner instance;

    void Start()
    {
        instance = this;
    }
    public static NPC Spawn (string npcId, Vector2 location, string scene)
    {
        GameObject npcObject = GameObject.Instantiate(
            instance.npcPrefab, 
            location, 
            Quaternion.identity, 
            SceneManager.GetSceneByName(scene).GetRootGameObjects()[0].transform
        );
        npcObject.GetComponent<NPC>().SetId(npcId);
        return npcObject.GetComponent<NPC>();
    }
}
