/**
 * An object which offers an item pickup to actors if CurrentlyPickuppable returns true.
 */
public interface IPickuppable
{
	// Whether the item pickup is currently available.
	bool CurrentlyPickuppable { get; }
	
	// The item the actor will receive.
	ItemStack ItemPickup { get; }
	
	// Called after an actor receives the item pickup
	// (for example, to destroy the item if it's a one-time pickup).
	void OnPickup();
}
