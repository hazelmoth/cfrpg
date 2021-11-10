using System.Collections.Generic;
using ContentLibraries;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ItemData = Items.ItemData;

namespace GUI
{
	public class TradeMenuManager : MonoBehaviour
    {
        private const string MenuTitle = "Trade";
        private const string BuyButtonLabel = "Buy";
        private const string SellButtonLabel = "Sell";
        private const string NothingToBuyMsg = "This trader has nothing available for sale.";
        private const string NothingToSellMsg = "You have nothing this trader is willing to buy.";
        private const string NoCurrentTransactionMsg = "There is no ongoing trade.";
        private const string NoItemToDescribeMsg = "Hover over an item to see its description.";
        private const string PlayerBalanceLabel = "Current Balance";
        private const string TraderBalanceLabel = "Trader Balance";
        private const string TransactionTotalLabel = "Transaction";

        private static TradeMenuManager instance;

        [SerializeField] private GameObject listItemPrefab = null;
        [SerializeField] private Color activeTabColor = Color.black;
        [SerializeField] private Color inactiveTabColor = Color.black;
        [SerializeField] private TextMeshProUGUI menuTitle = null;
        [SerializeField] private TextMeshProUGUI buyButtonText = null;
        [SerializeField] private TextMeshProUGUI sellButtonText = null;
        [SerializeField] private GameObject itemListContent = null;
        [SerializeField] private TextMeshProUGUI noItemsAvailableMessage = null;
        [SerializeField] private Image itemInfoIcon = null;
        [SerializeField] private TextMeshProUGUI itemInfoTitle = null;
        [SerializeField] private TextMeshProUGUI itemInfoDescription = null;
        [SerializeField] private TextMeshProUGUI playerBalanceText = null;
        [SerializeField] private TextMeshProUGUI traderBalanceText = null;
        [SerializeField] private TextMeshProUGUI transactionTotalText = null;

        private bool inSellTab = false;
        private TradeTransaction currentTransaction;

        private void Start()
        {
            instance = this;
            menuTitle.text = MenuTitle;
            buyButtonText.text = BuyButtonLabel;
            sellButtonText.text = SellButtonLabel;
            PlayerInteractionManager.OnTradeWithTrader += OnInitiateTrading;
        }

        public static void HandleItemMouseEnter(TradeListItem item)
        {
            instance.FillItemInfoPanel(item.itemId);
        }

        public static void HandleItemMouseExit()
        {
            instance.ClearItemInfoPanel();
        }

        public static void HandleItemChanged(TradeListItem item)
        {
            if (instance.inSellTab)
            {
                instance.currentTransaction.itemSells[item.itemId] = item.Quantity;
            }
            else
            {
                instance.currentTransaction.itemPurchases[item.itemId] = item.Quantity;
            }
            instance.UpdateBalanceDisplays();
        }

        public void ConfirmTradeButton()
        {
            if (TradeSystem.ActivateTrade(currentTransaction))
            {
                currentTransaction = null;
                UIManager.CloseAllMenus();
            }
        }

        public void CancelTradeButton()
        {
            currentTransaction = null;
            UIManager.CloseAllMenus();
        }

        public void ResetTradeButton()
        {
            currentTransaction = new TradeTransaction(currentTransaction.customerActorId, currentTransaction.vendorActorId);
            PopulateItemList();
            UpdateBalanceDisplays();
        }

        public void SwitchToBuyTab()
        {
            inSellTab = false;
            buyButtonText.color = activeTabColor;
            sellButtonText.color = inactiveTabColor;
            PopulateItemList();
        }

        public void SwitchToSellTab()
        {
            inSellTab = true;
            buyButtonText.color = inactiveTabColor;
            sellButtonText.color = activeTabColor;
            PopulateItemList();
        }

        private void OnInitiateTrading(Actor vendor)
        {
            currentTransaction = new TradeTransaction(PlayerController.PlayerActorId, vendor.ActorId);
            SwitchToBuyTab();
            UpdateBalanceDisplays();
        }

        private void PopulateItemList()
        {
            noItemsAvailableMessage.text = "";

            // Clear all existing list entries
            foreach (Transform existing in itemListContent.transform)
            {
                Destroy(existing.gameObject);
            }

            // Check if somehow the menu is active but we're not in a transaction
            if (currentTransaction == null)
            {
                noItemsAvailableMessage.text = NoCurrentTransactionMsg;
                return;
            }

            // Tracks list items we have already created, so we can increase quantity available when multiple items have the same ID
            Dictionary<string, TradeListItem> created = new Dictionary<string, TradeListItem>();

            if (inSellTab)
            {
                ActorData customer = ActorRegistry.Get(currentTransaction.customerActorId).data;
                foreach (ItemStack item in customer.Inventory.GetAllItems())
                {
                    // If we already have an item in the list with this ID, just increase the quantity available of that item
                    if (created.ContainsKey(item.Id))
                    {
                        created[item.Id].SetAvailableQuantity(created[item.Id].AvailableQuantity + item.Quantity);
                    }
                    else
                    {
                        GameObject newListing = Instantiate(listItemPrefab, itemListContent.transform, false);
                        TradeListItem listingData = newListing.GetComponent<TradeListItem>();
                        int sellPrice = TradeSystem.GetItemSellPrice(item.Id, currentTransaction.customerActorId, currentTransaction.vendorActorId);
                        listingData.SetItem(item.Id, sellPrice, item.Quantity);
                        if (currentTransaction.itemSells.ContainsKey(item.Id))
                        {
                            listingData.SetQuantity(currentTransaction.itemSells[item.Id]);
                        }
                        created.Add(item.Id, listingData);
                    }
                }
                if (customer.Inventory.GetAllItems().Count == 0)
                {
                    noItemsAvailableMessage.text = NothingToSellMsg;
                }
            }
            else
            {
                foreach (string itemId in TradeSystem.GetItemsForSale(currentTransaction.vendorActorId).Keys)
                {
                    // If we already have an item in the list with this ID, just increase the quantity available of that item
                    if (created.ContainsKey(itemId))
                    {
                        created[itemId].SetAvailableQuantity(created[itemId].AvailableQuantity + TradeSystem.GetItemsForSale(currentTransaction.vendorActorId)[itemId]);
                    }
                    else
                    {
                        GameObject newListing = Instantiate(listItemPrefab, itemListContent.transform, false);
                        TradeListItem listingData = newListing.GetComponent<TradeListItem>();
                        int price = TradeSystem.GetItemPurchasePrice(itemId, currentTransaction.customerActorId, currentTransaction.vendorActorId);

                        listingData.SetItem(itemId, price, TradeSystem.GetItemsForSale(currentTransaction.vendorActorId)[itemId]);
                        if (currentTransaction.itemPurchases.ContainsKey(itemId))
                        {
                            listingData.SetQuantity(currentTransaction.itemPurchases[itemId]);
                        }
                        created.Add(itemId, listingData);
                    }
                }
                if (TradeSystem.GetItemsForSale(currentTransaction.vendorActorId).Count == 0)
                {
                    noItemsAvailableMessage.text = NothingToBuyMsg;
                }
            }
        }

        private void FillItemInfoPanel(string itemId)
        {
            ItemData item = ContentLibrary.Instance.Items.Get(itemId);
            itemInfoIcon.color = Color.white;
            itemInfoTitle.text = item.GetItemName(itemId);
            itemInfoDescription.text = item.Description;
            itemInfoIcon.sprite = item.GetIcon(itemId);
        }

        private void ClearItemInfoPanel()
        {
            itemInfoTitle.text = "";
            itemInfoIcon.color = Color.clear;
            itemInfoDescription.text = NoItemToDescribeMsg;
        }

        private void UpdateBalanceDisplays()
        {
            if (currentTransaction == null)
            {
                return;
            }
            playerBalanceText.text = PlayerBalanceLabel + ": $" + ActorRegistry.Get(currentTransaction.customerActorId).data.Wallet.Balance;
            traderBalanceText.text = TraderBalanceLabel + ": $" + ActorRegistry.Get(currentTransaction.vendorActorId).data.Wallet.Balance;

            string transactionNumString = currentTransaction.TransactionTotal.ToString();
            if (transactionNumString.Contains("-"))
            {
                // If there's a negative sign, put the dollar sign after it
                transactionNumString = transactionNumString.Insert(1, "$");
            }
            else
            {
                transactionNumString = transactionNumString.Insert(0, "$");
            }

            if (currentTransaction.TransactionTotal > 0)
            {
                // Prepend a plus if the player is gaining money
                transactionNumString = "+" + transactionNumString;
            }
            transactionTotalText.text = TransactionTotalLabel + ": " + transactionNumString;
        }
    }
}
