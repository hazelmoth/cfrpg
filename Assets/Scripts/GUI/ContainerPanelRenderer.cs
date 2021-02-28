using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    // Handles setting up various layouts for the container window
    public class ContainerPanelRenderer : MonoBehaviour
    {
        private const float RowSpacing = 10f;
        private const float BaseHeight = 140f;
        private const int MAXInvRowSize = 6;
        private const int ContainerCapacity = 100;
        private readonly Vector2 startPivot = new Vector2(56.1f, -109.2f);

        [SerializeField] private GameObject invSlotPrefab;
        [SerializeField] private GameObject slotGridPrefab;
        [SerializeField] private RectTransform backgroundPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private GameObject elementParent;

        private IContainer current;
        private List<GameObject> labels;
        private float slotGridHeight;
        public GameObject[] Slots { get; private set; }

        private void Start()
        {
            slotGridHeight = slotGridPrefab.GetComponent<RectTransform>().rect.height;
        }

        // Renders the contents of the given container as a normal grid layout.
        public void RenderNormalContainer(IContainer container)
        {
            SetTitle(container.Name);
            Clear();

            Slots = new GameObject[ContainerCapacity]; // Probably enough, right?
            GameObject grid = Instantiate(slotGridPrefab, elementParent.transform);
            Vector2 pivot = startPivot;
            grid.GetComponent<RectTransform>().anchoredPosition = pivot;
            var gridTransform = slotGridPrefab.GetComponent<RectTransform>();
            // Count a row at a time
            for (int r = 0; r < container.SlotCount; r += MAXInvRowSize)
            {
                for (int i = 0; i < MAXInvRowSize && i+r < container.SlotCount; i++)
                {
                    GameObject slot = Instantiate(invSlotPrefab, grid.transform);
                    Slots[r+i] = slot;

                    // Render the actual item in the slot
                    ItemStack item = container.Get(r+i);

                    if (item != null)
                    {
                        SetSlotDisplay(r+i, item.GetData().Icon, item.quantity);
                    }
                    else
                    {
                        SetSlotDisplay(r + i, null, 0);
                    }
                }
                pivot.y -= gridTransform.rect.height;
                pivot.y -= RowSpacing;
            }

            // Resize the grid to fit all the elements.
            gridTransform.anchorMin = new Vector2(gridTransform.anchorMin.x, pivot.y);
                
            // Resize the container window to fit the grid nicely.
            SetBackgroundSize(pivot);
        }
        
        // Renders the given list of layout elements as a custom container layout.
        public void RenderCustomLayout(ICustomLayoutContainer container)
        {
            current = container;
            // Add a listener to re-render this container when it changes
            container.SetUpdateListener(changedContainer =>
            {
                if (changedContainer == current)
                {
                    RenderCustomLayout(container);
                }
            });
            
            SetTitle(container.Name);
            Clear();

            List<IContainerLayoutElement> layout = container.GetLayoutElements();
            Slots = new GameObject[ContainerCapacity];
            labels = new List<GameObject>();

            Vector2 pivot = startPivot;
            foreach (IContainerLayoutElement element in layout)
            {
                if (element is ContainerLayoutInvArray inv)
                {
                    GameObject grid = Instantiate(slotGridPrefab, elementParent.transform);
                    grid.GetComponent<RectTransform>().anchoredPosition = pivot;
                    for (int i = inv.startIndex; i <= inv.endIndex; i++)
                    {
                        GameObject slot = Instantiate(invSlotPrefab, grid.transform);
                        Slots[i] = slot;
                        if (i - inv.startIndex + 1 >= MAXInvRowSize && i < inv.endIndex)
                        {
                            pivot.y -= slotGridHeight;
                            pivot.y -= RowSpacing;
                        }
                        
                        // Render the actual item in the slot
                        ItemStack item = container.Get(i);
                        if (item != null)
                        {
                            SetSlotDisplay(i, item.GetData().Icon, item.quantity);
                        }
                        else
                        {
                            SetSlotDisplay(i, null, 0);
                        }
                    }
                    pivot.y -= slotGridHeight;
                    pivot.y -= RowSpacing;
                }
                else
                {
                    GameObject created = element.Create(out float pivotDelta);
                    created.transform.SetParent(elementParent.transform, false);
                    created.GetComponent<RectTransform>().anchoredPosition = pivot;
                    pivot.y -= pivotDelta;
                    pivot.y -= RowSpacing;
                    labels.Add(created);
                }
                SetBackgroundSize(pivot);
            }
        }

        // Resizes the container window background to accomodate a layout ending on the given pivot.
        private void SetBackgroundSize(Vector2 endPivot)
        {
            backgroundPanel.sizeDelta = new Vector2(backgroundPanel.sizeDelta.x, BaseHeight + (startPivot.y - endPivot.y));
        }

        public void SetTitle (string title)
        {
            titleText.text = title;
        }
        
        // Renders the given sprite and given item quantity in the slot of the given index. If sprite is null, hides the
        // sprite of the given slot. Doesn't display item quantity if it equals zero or one.
        private void SetSlotDisplay (int i, Sprite image, int quantity)
        {
            Debug.Assert(Slots != null);
            
            GameObject slot = Slots[i];
            Debug.Assert(slot != null);

            InventoryIconInteractable icon = slot.GetComponentInChildren<InventoryIconInteractable>();
            if (image == null)
            {
                icon.SetVisible(false);
                icon.SetQuantityText("");
                return;
            }
            icon.SetVisible(true);
            if (quantity > 1)
            {
                icon.SetQuantityText(quantity.ToString());
            }
            else
            {
                icon.SetQuantityText("");
            }
            icon.GetComponent<Image>().sprite = image;
        }

        // Destroys all children of the container background except for the title text.
        public void Clear()
        {
            foreach (Transform child in backgroundPanel.transform)
            {
                if (child.gameObject.GetInstanceID() != titleText.gameObject.GetInstanceID())
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}