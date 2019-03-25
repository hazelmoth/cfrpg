using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] GameObject npcPrefab = null;
    static NPCSpawner instance;

    void Start()
    {
        instance = this;
    }
    public static void Spawn (NPCData npc, Vector2 location)
    {
    GameObject NPC = GameObject.Instantiate(instance.npcPrefab);
    }
}
