using System.Collections.Generic;
using ContentLibraries;
using Items;
using ItemData = Items.ItemData;

public static class TradeSystem
{
	// The multiplier by item value for a customer selling items
	private const float SellPriceMultiplier = 0.9f;
	// And for a customer buying items
	private const float PurchasePriceMultiplier = 1.1f;

	// Performs the given trade, exchanging items and money. Assumes that all items with a given ID are identical.
	public static bool ActivateTrade (TradeTransaction trade)
	{
		if (!CustomerHasSufficientFunds(trade) || !VendorHasSufficientFunds(trade))
		{
			return false;
		}
		ActorData vendor = ActorRegistry.Get(trade.vendorActorId).data;
		ActorData customer = ActorRegistry.Get(trade.customerActorId).data;

		trade.vendorWallet.AddBalance(-trade.TransactionTotal);
		customer.Wallet.AddBalance(trade.TransactionTotal);

		foreach (string itemId in trade.itemPurchases.Keys)
		{
			trade.vendorInventory.AttemptRemove(itemId, trade.itemPurchases[itemId]);
			for (int i = 0; i < trade.itemPurchases[itemId]; i++)
			{
				customer.Inventory.AttemptAddItem(new ItemStack(itemId, 1));
			}
		}
		foreach (string itemId in trade.itemSells.Keys)
		{
			customer.Inventory.Remove(itemId, trade.itemSells[itemId]);
			for (int i = 0; i < trade.itemSells[itemId]; i++)
			{
				trade.vendorInventory.AttemptAdd(itemId, 1);
			}
		}
		return true;
	}

	public static bool CustomerHasSufficientFunds (TradeTransaction trade)
	{
		return (ActorRegistry.Get(trade.customerActorId).data.Wallet.Balance >= -trade.TransactionTotal);
	}

	public static bool VendorHasSufficientFunds(TradeTransaction trade)
	{
		return trade.vendorWallet.Balance >= trade.TransactionTotal;
	}

	/// Price for the given customer to buy the given item from the given vendor
	public static int GetItemPurchasePrice(string itemId, string customerActorId, string vendorActorId)
	{
		ActorData vendor = ActorRegistry.Get(vendorActorId).data;
		ItemData item = ContentLibrary.Instance.Items.Get(itemId);

		return (int)(item.BaseValue * PurchasePriceMultiplier);
	}

	/// Price for the given customer to sell the given item to the given vendor
	public static int GetItemSellPrice (string itemId, string customerActorId, string vendorActorId)
	{
		ItemData item = ContentLibrary.Instance.Items.Get(itemId);
		// Reduce the price when the customer sells items (so the vendor can make a living)
		return(int)(item.BaseValue * SellPriceMultiplier);
	}

	/// Maps Item IDs to the number of available items in the given container.
	public static Dictionary<string, int> GetItemsForSale(IContainer itemSource)
	{
		Dictionary<string, int> items = new();

		for (int i = 0; i < itemSource.SlotCount; i++)
		{
			ItemStack item = itemSource.Get(i);
			if (item == null) continue;

			if (items.ContainsKey(item.Id)) items[item.Id] += item.Quantity;
			else items[item.Id] = item.Quantity;
		}
		return items;
	}
}
