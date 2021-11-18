using System;

/// An InventorySlot is capable of holding a single ItemStack.
/// May accept only certain types of Items.
public class InventorySlot
{
    private readonly Func<string, bool> itemAcceptor;

    /// The ItemStack currently in this slot.
    /// Note that this can be set regardless of what this slot is willing to accept.
    public ItemStack Contents { get; set; }

    /// True if this slot is not holding an item.
    public bool Empty => Contents == null;

    /// Constructs a slot that will accept any item.
    public InventorySlot()
    {
        itemAcceptor = _ => true;
    }

    /// Constructs a slot that will accept items based on the given acceptor.
    public InventorySlot(Func<string, bool> itemAcceptor)
    {
        this.itemAcceptor = itemAcceptor;
    }

    /// Whether this slot is willing to hold the given item.
    public virtual bool CanHoldItem(string itemId)
    {
        return itemAcceptor(itemId);
    }
}
