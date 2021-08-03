namespace Items
{
	// For items that can select and interact with the tile immediately
	// on front of the user, e.g. for plopping things down
	public interface IPloppable
	{
		/// Uses this IPloppable on the given tile, and returns a new ItemStack representing
		/// the item after being plopped, or null if it is destroyed.
		ItemStack Use(TileLocation target, ItemStack instance);

		/// Whether a tile selector should be visible for the given instance.
		bool VisibleTileSelector(ItemStack instance);
	}
}