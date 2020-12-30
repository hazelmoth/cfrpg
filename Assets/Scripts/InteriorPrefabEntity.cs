using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script must be on entities included as part of interior prefabs, in
// order for them to be properly recognized as entities.
public class InteriorPrefabEntity : MonoBehaviour
{
    [SerializeField] public string entityID;

    // Start is called before the first frame update
    void Start()
    {
        if (entityID == null || entityID == "") Debug.LogError("Entity ID should have been set in the inspector!", this);
    }
}
