using System.Collections.Generic;

/// Represents a transaction of items and money between a vendor and a customer.
public class TradeTransaction
{
	public readonly string vendorActorId;
	public readonly string customerActorId;
	public readonly IDictionary<string, int> itemPurchases;
	public readonly IDictionary<string, int> itemSells;

	public TradeTransaction (string buyerActorId, string vendorActorId)
	{
		this.vendorActorId = vendorActorId;
		this.customerActorId = buyerActorId;

		itemPurchases = new Dictionary<string, int>();
		itemSells = new Dictionary<string, int>();
	}

	// Sets how many of the given item the buyer will buy from the vendor in this transaction.
	public void SetItemPurchaseQuantity (string item, int quantity)
	{
		if (itemPurchases.ContainsKey(item))
		{
			itemPurchases[item] = quantity;
		}
		else
		{
			itemPurchases.Add(item, quantity);
		}
	}

	// Sets how many of the given item the customer will sell *to* the vendor in this transaction.
	public void SetItemSellQuantity(string item, int quantity)
	{
		if (itemSells.ContainsKey(item))
		{
			itemSells[item] = quantity;
		}
		else
		{
			itemSells.Add(item, quantity);
		}
	}

	/// Positive if the customer gains money, negative if the customer loses money.
	public int TransactionTotal
	{
		get
		{
			int total = 0;
			foreach (string item in itemPurchases.Keys)
			{
				total -= TradeSystem.GetItemPurchasePrice(item, customerActorId, vendorActorId) * itemPurchases[item];
			}
			foreach (string item in itemSells.Keys)
			{
				total += TradeSystem.GetItemSellPrice(item, customerActorId, vendorActorId) * itemSells[item];
			}
			return total;
		}
	}
}
