using System.Collections.Generic;

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

		vendor.Wallet.AddBalance(-trade.TransactionTotal);
		customer.Wallet.AddBalance(trade.TransactionTotal);

		foreach (string itemId in trade.itemPurchases.Keys)
		{
			vendor.Inventory.Remove(itemId, trade.itemPurchases[itemId]);
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
				vendor.Inventory.AttemptAddItem(new ItemStack(itemId, 1));
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
		return (ActorRegistry.Get(trade.vendorActorId).data.Wallet.Balance >= trade.TransactionTotal);
	}

	// Price for the given customer to buy the given item from the given vendor
	public static int GetItemPurchasePrice(string itemId, string customerActorId, string vendorActorId)
	{
		ActorData vendor = ActorRegistry.Get(vendorActorId).data;
		ItemData item = ContentLibrary.Instance.Items.Get(itemId);

		return (int)(item.BaseValue * PurchasePriceMultiplier);
	}

	// Price for the given customer to sell the given item to the given vendor
	public static int GetItemSellPrice (string itemId, string customerActorId, string vendorActorId)
	{
		ItemData item = ContentLibrary.Instance.Items.Get(itemId);
		// Reduce the price when the customer sells items (so the vendor can make a living)
		return(int)(item.BaseValue * SellPriceMultiplier);
	}

	// Maps Item IDs to the number the trader has available.
	public static Dictionary<string, int> GetItemsForSale(string actorId)
	{
		ActorData actor = ActorRegistry.Get(actorId).data;
		Dictionary<string, int> items = new Dictionary<string, int>();

		foreach (ItemStack item in actor.Inventory.GetAllItems())
		{
			if (items.ContainsKey(item.id))
			{
				items[item.id] += item.quantity;
			}
			else
			{
				items[item.id] = item.quantity;
			}
		}
		return items;
	}
}
