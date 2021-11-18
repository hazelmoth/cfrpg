using System;
using ContentLibraries;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GUI.ContainerLayoutElements
{
    /// A button with a text label.
    public class ContainerLayoutButton : IContainerLayoutElement
    {
        private const string PrefabId = "button";
        private readonly Func<string> labelTextSupplier;
        private readonly Action onClick;

        public ContainerLayoutButton(Func<string> labelTextSupplier, Action onClick)
        {
            this.labelTextSupplier = labelTextSupplier;
            this.onClick = onClick;
        }

        GameObject IContainerLayoutElement.Create(out float pivotDelta)
        {
            GameObject prefab = ContentLibrary.Instance.ContainerLayoutElementPrefabs.Get(PrefabId);
            GameObject created = Object.Instantiate(prefab);

            created.GetComponent<Button>().onClick.AddListener(() => onClick());

            GameObject labelObject = created.GetComponentInChildren<TextMeshProUGUI>().gameObject;
            labelObject.GetComponent<TextMeshProUGUI>().text = labelTextSupplier.Invoke();
            labelObject.AddComponent<TextUpdater>().SetValueSupplier(labelTextSupplier);

            pivotDelta = created.GetComponent<RectTransform>().rect.height;
            return created;
        }
    }
}
