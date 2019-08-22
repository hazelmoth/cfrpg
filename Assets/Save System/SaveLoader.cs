using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void LoadSave(WorldSave save)
    {
        WorldMapManager.LoadMap(save.worldMap.ToNonSerializable());
    }
}
