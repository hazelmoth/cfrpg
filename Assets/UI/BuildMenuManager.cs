using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuManager : MonoBehaviour
{

    [SerializeField] GameObject entityMenuContent = null;
    [SerializeField] GameObject entityMenuItemPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        //TEST
        List<EntityData> entities = new List<EntityData>();
        foreach (string id in EntityLibrary.GetEntityIdList())
            entities.Add(EntityLibrary.GetEntityFromID(id));
        PopulateEntityMenu(entities);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void PopulateEntityMenu (List<EntityData> entities)
    {
        foreach (EntityData entity in entities)
        {
            GameObject newMenuItem = GameObject.Instantiate(entityMenuItemPrefab);
            newMenuItem.GetComponent<EntityMenuItem>().SetEntity(entity);
            newMenuItem.transform.SetParent(entityMenuContent.transform);
        }
    }
}
