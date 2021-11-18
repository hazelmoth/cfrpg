/// Represents an object that stores items in slots.
public interface IContainer
{
	/// The name of the container to be displayed to the player.
	string Name { get; }

	/// How many item slots this container has.
	int SlotCount { get; }

	/// Returns the item in the specified slot, or null if the slot is empty.
	ItemStack Get(int slot);

	/// Sets the contents of the specified slot to the given item.
	/// Note that this does not check if the item is valid for the slot.
	void Set(int slot, ItemStack item);

	/// Whether the specified slot is willing to hold the item with the specified ID.
	bool AcceptsItemType(string itemId, int slot);
}
