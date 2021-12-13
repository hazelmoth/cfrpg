using System.Collections.Generic;

/// Represents a transaction of items and money between a vendor and a customer.
public class TradeTransaction
{
	public readonly string vendorActorId;
	public readonly string customerActorId;
	public readonly IContainer vendorInventory;
	public readonly IWallet vendorWallet;
	public readonly IDictionary<string, int> itemPurchases;
	public readonly IDictionary<string, int> itemSells;

	public TradeTransaction (string customerActorId, string vendorActorId, IContainer vendorInv, IWallet vendorWallet)
	{
		this.vendorActorId = vendorActorId;
		this.customerActorId = customerActorId;
		this.vendorInventory = vendorInv;
		this.vendorWallet = vendorWallet;

		itemPurchases = new Dictionary<string, int>();
		itemSells = new Dictionary<string, int>();
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
