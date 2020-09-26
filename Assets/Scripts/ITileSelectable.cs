public interface ITileSelectable
{
	float TileSelectorRange { get; }

	bool VisibleTileSelector { get; }

	void Use(TileLocation target);
}
