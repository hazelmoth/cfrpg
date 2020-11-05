using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    // Handles setting up various layouts for the container window
    public class ContainerPanelRenderer : MonoBehaviour
    {
        private const float rowSpacing = 10f;
        private const float baseHeight = 140f;
        private const int maxInvRowSize = 6;
        private readonly Vector2 startPivot = new Vector2(56.1f, -109.2f);

        [SerializeField] private GameObject invSlotPrefab = null;
        [SerializeField] private GameObject slotGridPrefab = null;
        [SerializeField] private TextMeshProUGUI labelPrefab = null;
        [SerializeField] private RectTransform backgroundPanel = null;
        [SerializeField] private TextMeshProUGUI titleText = null;
        [SerializeField] private GameObject elementParent = null;

        public GameObject[] Slots { get; private set; }
        private List<GameObject> labels;

        public void RenderCustomLayout(List<IContainerLayoutElement> layout)
        {
            Clear();
            Slots = new GameObject[100]; // Probably enough, right?
            labels = new List<GameObject>();

            Vector2 pivot = startPivot;
            foreach (IContainerLayoutElement element in layout)
            {
                if (element is ContainerLayoutLabel label)
                {
                    GameObject created = GameObject.Instantiate(labelPrefab.gameObject, elementParent.transform);
                    created.GetComponent<RectTransform>().anchoredPosition = pivot;
                    created.GetComponent<TextMeshProUGUI>().text = label.text;
                    pivot.y -= labelPrefab.rectTransform.rect.height;
                    pivot.y -= rowSpacing;
                    labels.Add(created);
                }
                else if (element is ContainerLayoutInvArray inv)
                {
                    GameObject grid = GameObject.Instantiate(slotGridPrefab, elementParent.transform);
                    grid.GetComponent<RectTransform>().anchoredPosition = pivot;
                    for (int i = inv.startIndex; i <= inv.endIndex; i++)
                    {
                        GameObject slot = GameObject.Instantiate(invSlotPrefab, grid.transform);
                        Slots[i] = slot;
                        if (i - inv.startIndex + 1 >= maxInvRowSize && i < inv.endIndex)
                        {
                            pivot.y -= slotGridPrefab.GetComponent<RectTransform>().rect.height;
                            pivot.y -= rowSpacing;
                        }
                        SetSlotDisplay(i, null, 0);
                    }
                    pivot.y -= slotGridPrefab.GetComponent<RectTransform>().rect.height;
                    pivot.y -= rowSpacing;
                }
                else
                {
                    Debug.LogError("WTF is this element? I don't know what to do with this " + element.GetType().Name + ".");
                }
                backgroundPanel.sizeDelta = new Vector2(backgroundPanel.sizeDelta.x, baseHeight + (startPivot.y - pivot.y));
            }
        }

        public void SetTitle (string title)
        {
            titleText.text = title;
        }

        public void SetSlotDisplay (int i, Sprite image, int quantity)
        {
            if (Slots == null) Slots = new GameObject[100];

            GameObject slot = Slots[i];
            InventoryIconInteractable icon = slot.GetComponentInChildren<InventoryIconInteractable>();
            if (image == null)
            {
                icon.SetVisible(false);
                icon.SetQuantityText("");
                return;
            }
            icon.SetVisible(true);
            icon.SetQuantityText(quantity.ToString());
            slot.GetComponentInChildren<Image>().sprite = image;
        }

        public void Clear()
        {
            if (Slots == null)  Slots = new GameObject[100];
            if (labels == null) labels = new List<GameObject>();

            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i] != null)
                {
                    Destroy(Slots[i]);
                }
            }
            foreach(GameObject label in labels)
            {
                Destroy(label);
            }
        }
    }
}