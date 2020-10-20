namespace Items
{
	// For items that can select and interact with the tile immediately
	// on front of the user, e.g. for plopping things down
	public interface IPloppable
	{
		void Use(TileLocation target, ItemStack instance);

		bool VisibleTileSelector(ItemStack instance);
	}
}