using System;
using System.Collections.Generic;
using ActorComponents;
using ActorTemplates;
using ContentLibraries;
using MyBox;
using UnityEngine;

/// A template that generates a randomized actor with the full CFRPG set of components.
[Serializable]
[CreateAssetMenu]
public class AdvancedRandomizedActorTemplate : ActorTemplate
{
    public string templateId;
    public float femaleChance = 0.5f;
    public List<string> races;
    public List<string> hairs;
    public float hatChance = 0.5f;
    public List<string> hats;
    public List<string> shirts;
    public List<string> pants;
    public List<string> personalities;
    public List<string> professions;
    [Separator]
    public int minMoney = 5;
    public int maxMoney = 50;
    public CompoundWeightedTable inventoryTable;

    public override string Id => templateId;

    public override ActorData CreateActor(Func<string, bool> isIdAvailable, out string id)
    {
	    ActorData created = Generate(isIdAvailable);
	    id = created.ActorId;
	    return created;
    }

    private ActorData Generate(Func<string, bool> isIdAvailable)
	{
		string race = races.Count > 0 ? races.PickRandom() : null;
		string personality = personalities.Count > 0 ? personalities.PickRandom() : null;
		string hat = hats.Count > 0 ? hats.PickRandom() : null;
		string shirt = shirts.Count > 0 ? shirts.PickRandom() : null;
		string pants = this.pants.Count > 0 ? this.pants.PickRandom() : null;
		string hair = hairs.Count > 0 ? hairs.PickRandom() : null;
		string profession = professions.Count > 0 ? professions.PickRandom() : null;

		IActorRace raceData = ContentLibrary.Instance.Races.Get(race);
		if (raceData == null)
		{
			Debug.LogError($"Couldn't generate actor of race \"{race}\"; race data not found.");
			return null;
		}
		if (race != null && !raceData.SupportsHair)
		{
			hair = null;
		}

		// chance of no hat
		System.Random random = new();
		if (random.NextDouble() > hatChance)
			hat = null;

		Gender gender = Gender.Male;
		if (random.NextDouble() < femaleChance)
			gender = Gender.Female;

		ActorInventory inv = new();
		inv.SetItemInSlot(0, InventorySlotType.Hat,   hat != null ? new ItemStack(hat,     1) : null);
		inv.SetItemInSlot(0, InventorySlotType.Shirt, shirt != null ? new ItemStack(shirt, 1) : null);
		inv.SetItemInSlot(0, InventorySlotType.Pants, pants != null ? new ItemStack(pants, 1) : null);
		inventoryTable.Pick().ForEach(itemId => inv.AttemptAddItem(new ItemStack(itemId, 1)));

		string name = NameGenerator.Generate(gender);
		string id = CreateUniqueId(name, isIdAvailable);
		float maxHealth = ContentLibrary.Instance.Races.Get(race).MaxHealth;
		int money = UnityEngine.Random.Range(minMoney, maxMoney + 1);

		return new ActorData(
			id,
			race,
			new ActorName(name),
			new ActorPersonality(personality),
			new ActorGender(gender),
			new ActorHair(hair),
			new ActorHealth(maxHealth, maxHealth),
			inv,
			new ActorWallet(money),
			new FactionStatus(null),
			new ActorProfession(profession));
	}
}
