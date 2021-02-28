using System;
using TMPro;
using UnityEngine;

// Represents a line of text in a container layout, which can be updated automatically
// via a given supplier.
public class ContainerLayoutLabel : IContainerLayoutElement
{
    private const string PrefabId = "label";
    private readonly Func<String> valSupplier;

    public ContainerLayoutLabel(Func<String> valSupplier)
    {
        this.valSupplier = valSupplier;
    }
    
    public GameObject Create(out float pivotDelta)
    {
        GameObject prefab = ContentLibrary.Instance.ContainerLayoutElementPrefabs.Get(PrefabId);
        GameObject created = GameObject.Instantiate(prefab);
        created.GetComponent<TextMeshProUGUI>().text = valSupplier.Invoke();
        created.AddComponent<TextUpdater>().SetValueSupplier(valSupplier);
        pivotDelta = created.GetComponent<RectTransform>().rect.height;
        return created;
    }
}
