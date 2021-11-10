using Crafting;

public interface ICraftingStation
{
	string WorkstationName { get; }
	CraftingEnvironment Environment { get; }
}
