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
			TilemapInterface.ScenePosToWorldPos(location, scene), 
            Quaternion.identity, 
			SceneObjectManager.GetSceneObjectFromId(scene).transform
        );
        npcObject.GetComponent<NPC>().SetId(npcId);
        return npcObject.GetComponent<NPC>();
    }
}
