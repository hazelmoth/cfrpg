using System;
using ContentLibraries;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GUI.ContainerLayoutElements
{
    /// Represents a line of text in a container layout, which can be updated automatically
    /// via a given supplier.
    public class ContainerLayoutLabel : IContainerLayoutElement
    {
        private const string PrefabId = "label";
        private readonly Func<string> valSupplier;

        public ContainerLayoutLabel(Func<string> valSupplier)
        {
            this.valSupplier = valSupplier;
        }
    
        public GameObject Create(out float pivotDelta)
        {
            GameObject prefab = ContentLibrary.Instance.ContainerLayoutElementPrefabs.Get(PrefabId);
            GameObject created = Object.Instantiate(prefab);

            created.GetComponent<TextMeshProUGUI>().text = valSupplier.Invoke();
            created.AddComponent<TextUpdater>().SetValueSupplier(valSupplier);

            pivotDelta = created.GetComponent<RectTransform>().rect.height;
            return created;
        }
    }
}
