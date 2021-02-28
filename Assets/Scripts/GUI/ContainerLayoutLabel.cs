using System;
using TMPro;
using UnityEngine;

public class ContainerLayoutLabel : IContainerLayoutElement
{
    private const string PrefabId = "label";
    private Func<String> valProvider;
    
    public ContainerLayoutLabel(Func<String> valProvider)
    {
        this.valProvider = valProvider;
    }
    
    public GameObject Create(out float pivotDelta)
    {
        GameObject prefab = ContentLibrary.Instance.ContainerLayoutElementPrefabs.Get(PrefabId);
        GameObject created = GameObject.Instantiate(prefab);
        created.GetComponent<TextMeshProUGUI>().text = valProvider.Invoke();
        pivotDelta = created.GetComponent<RectTransform>().rect.height;
        return created;
    }
}
