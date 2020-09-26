using ActorComponents;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private GameObject listItemPrefab;
        [SerializeField] private Color activeTabColor;
        [SerializeField] private Color inactiveTabColor;
        [SerializeField] private TextMeshProUGUI menuTitle;
        [SerializeField] private TextMeshProUGUI buyButtonText;
        [SerializeField] private TextMeshProUGUI sellButtonText;
        [SerializeField] private GameObject itemListContent;
        [SerializeField] private TextMeshProUGUI noItemsAvailableMessage;
        [SerializeField] private Image itemInfoIcon;
        [SerializeField] private TextMeshProUGUI itemInfoTitle;
        [SerializeField] private TextMeshProUGUI itemInfoDescription;
        [SerializeField] private TextMeshProUGUI playerBalanceText;
        [SerializeField] private TextMeshProUGUI traderBalanceText;
        [SerializeField] private TextMeshProUGUI transactionTotalText;

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
            instance.FillItemInfoPanel(ContentLibrary.Instance.Items.Get(item.itemId));
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

            int price;

            // Tracks list items we have already created, so we can increase quantity available when multiple items have the same ID
            Dictionary<string, TradeListItem> created = new Dictionary<string, TradeListItem>();

            if (inSellTab)
            {
                ActorData customer = ActorRegistry.Get(currentTransaction.customerActorId).data;
                foreach (Item item in customer.Inventory.GetAllItems())
                {
                    // If we already have an item in the list with this ID, just increase the quantity available of that item
                    if (created.ContainsKey(item.id))
                    {
                        created[item.id].SetAvailableQuantity(created[item.id].AvailableQuantity + item.quantity);
                    }
                    else
                    {
                        GameObject newListing = Instantiate(listItemPrefab, itemListContent.transform, false);
                        TradeListItem listingData = newListing.GetComponent<TradeListItem>();
                        price = TradeSystem.GetItemSellPrice(item.id, currentTransaction.customerActorId, currentTransaction.vendorActorId);
                        listingData.SetItem(item.id, price, item.quantity);
                        if (currentTransaction.itemSells.ContainsKey(item.id))
                        {
                            listingData.SetQuantity(currentTransaction.itemSells[item.id]);
                        }
                        created.Add(item.id, listingData);
                    }
                }
                if (customer.Inventory.GetAllItems().Count == 0)
                {
                    noItemsAvailableMessage.text = NothingToSellMsg;
                }
            }
            else
            {
                Trader trader = ActorRegistry.Get(currentTransaction.vendorActorId).data.GetComponent<Trader>();
                foreach (string itemId in trader.GetItemsForSale().Keys)
                {
                    // If we already have an item in the list with this ID, just increase the quantity available of that item
                    if (created.ContainsKey(itemId))
                    {
                        created[itemId].SetAvailableQuantity(created[itemId].AvailableQuantity + trader.GetItemsForSale()[itemId]);
                    }
                    else
                    {
                        GameObject newListing = Instantiate(listItemPrefab, itemListContent.transform, false);
                        TradeListItem listingData = newListing.GetComponent<TradeListItem>();
                        listingData.SetItem(itemId, trader.GetPurchasePrice(itemId), trader.GetItemsForSale()[itemId]);
                        if (currentTransaction.itemPurchases.ContainsKey(itemId))
                        {
                            listingData.SetQuantity(currentTransaction.itemPurchases[itemId]);
                        }
                        created.Add(itemId, listingData);
                    }
                }
                if (trader.GetItemsForSale().Count == 0)
                {
                    noItemsAvailableMessage.text = NothingToBuyMsg;
                }
            }
        }

        private void FillItemInfoPanel(ItemData item)
        {
            itemInfoIcon.color = Color.white;
            itemInfoTitle.text = item.ItemName;
            itemInfoDescription.text = item.Description;
            itemInfoIcon.sprite = item.ItemIcon;
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