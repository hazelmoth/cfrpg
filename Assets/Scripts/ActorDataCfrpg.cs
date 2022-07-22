using System.Collections.Generic;
using System.Linq;
using ActorComponents;
using ContentLibraries;

/// A fleshed-out ActorData template that is initialized with inventory and health.
public class ActorDataCfrpg : ActorData
{
	private List<ISaveable> components;

	public T GetComponent<T>() where T : class
	{
		return components.OfType<T>().FirstOrDefault();
	}

	public ActorDataCfrpg(
		string actorId,
		string raceId,
		string roleId,
		string actorName,
		string personality,
		Gender gender,
		string hairId,
		float maxHealth,
		InventoryData inventory,
		int walletMoney,
		int debt,
		FactionStatus factionStatus) : base(actorId, raceId)
	{

	}

	// The benefit of this class is that we can add properties to directly reference
	// components so classes that use certain components a lot (like behavior nodes) don't
	// have to cache the properties. But... that eliminates much of the flexibility of the
	// component-based approach. Should maybe get rid of this class and just make a practice
	// of clients generally referencing specific components rather than actors.
}
