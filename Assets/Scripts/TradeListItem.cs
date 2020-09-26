using GUI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeListItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI numberAvailableText;
    [SerializeField] private TMP_InputField quantityInput;

    public string itemId;

    public int Quantity { get; private set; }

    public int AvailableQuantity { get; private set; }

    private static int pointerEnterCount;
    

    public void SetItem (string itemId, int price, int numAvailable)
    {
        this.itemId = itemId;
        this.AvailableQuantity = numAvailable;
        ItemData data = ContentLibrary.Instance.Items.Get(itemId);
        nameText.text = data.ItemName;
        priceText.text = "$" + price.ToString();
        numberAvailableText.text = numAvailable.ToString();
        icon.sprite = data.ItemIcon;
    }

    public void ResetQuantity()
    {
        Quantity = 0;
        quantityInput.text = Quantity.ToString();
        ValidateQuantity();
        HandleQuantityChanged();
    }

    public void SetQuantity(int amount)
    {
        Quantity = amount;
        quantityInput.text = Quantity.ToString();
        ValidateQuantity();
        HandleQuantityChanged();
    }

    public void SetAvailableQuantity(int amount)
    {
        AvailableQuantity = amount;
        numberAvailableText.text = AvailableQuantity.ToString();
    }

    // Called by arrow buttons
    public void IncrementQuantity()
    {
        Quantity++;
        quantityInput.text = Quantity.ToString();
        ValidateQuantity();
        HandleQuantityChanged();
    }
    public void DecrementQuantity()
    {
        Quantity--;
        quantityInput.text = Quantity.ToString();
        ValidateQuantity();
        HandleQuantityChanged();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        pointerEnterCount++;
        TradeMenuManager.HandleItemMouseEnter(this);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        pointerEnterCount--;
        pointerEnterCount = Math.Max(pointerEnterCount, 0);
        if (pointerEnterCount == 0) {
            TradeMenuManager.HandleItemMouseExit();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pointerEnterCount = 0;
        quantityInput.onValueChanged.AddListener(OnInputFieldChanged);
    }

    private void OnInputFieldChanged(string text)
    {
        if (int.TryParse(text, out int val))
        {
            Quantity = val;
        }
        ValidateQuantity();
    }

    private void ValidateQuantity ()
    {
        if (Quantity <= 0)
        {
            Quantity = 0;
            quantityInput.text = "";
        }
        if (Quantity > AvailableQuantity)
        {
            Quantity = AvailableQuantity;
            quantityInput.text = Quantity.ToString();
        }
    }

    private void HandleQuantityChanged ()
    {
        TradeMenuManager.HandleItemChanged(this);
    }
}
