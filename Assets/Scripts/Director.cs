using System;
using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using ContinentMaps;
using JetBrains.Annotations;
using Popcron.Console;
using SettlementSystem;
using UnityEngine;
using Random = UnityEngine.Random;

/// Responsible for triggering events like traders arriving
public class Director : MonoBehaviour
{
    [UsedImplicitly]
    [Command("debugevents")]
    public static bool debug = false;
    /// A list of functions, each of which returns an event that is set to occur.
    private List<Func<PeriodicEvent>> periodicEvents;
    /// A list of functions, each of which returns an event that is set to occur.
    private List<Func<WeeklyEvent>> scheduledEvents;

    private History history;

    private void Start()
    {
        // Event list --------------------------------------------------------------------

        periodicEvents = new List<Func<PeriodicEvent>>
        {
            () => new PerRegionPeriodicEvent("natural_spawns", DoNaturalSpawns, 1f),
        };

        scheduledEvents = new List<Func<WeeklyEvent>>
        {
            // Stores refill at 12:00am Saturday and Wednesday
            () => new PerRegionWeeklyEvent("restock_stores_weekend", RefillShopStations, WeekDay.Saturday,  0),
            () => new PerRegionWeeklyEvent("restock_stores_midweek", RefillShopStations, WeekDay.Wednesday, 0),
            () => new PerRegionWeeklyEvent("settlers_arrive",        SpawnNewResidents,  WeekDay.Monday, 0.52f),
        };

        history = FindObjectOfType<History>();
        if (history == null) Debug.LogError("Couldn't find History object in scene!");
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameInitializer.InitializationFinished) return;

        // Check if any events should occur
        foreach (PeriodicEvent e in periodicEvents.Select(eventSupplier => eventSupplier.Invoke()))
        {
            Debug.Assert(e.daysBetweenOccurrences > 0, $"Event {e.Id} has a frequency of {e.daysBetweenOccurrences}");

            float daysSinceLast = TimeKeeper.DaysBetween(
                history.GetMostRecent(e.Id)?.time ?? ulong.MinValue,
                TimeKeeper.CurrentTick);

            if (daysSinceLast >= e.daysBetweenOccurrences)
            {
                e.action();
                history.LogEvent(e.Id);
                if (debug)
                {
                    Debug.Log($"Event occurred: {e.Id}");
                    NotificationManager.Notify($"Event occurred: {e.Id}");
                }
            }
        }

        foreach (WeeklyEvent e in scheduledEvents.Select(eventSupplier => eventSupplier.Invoke()))
        {
            ulong lastOccurrence = history.GetMostRecent(e.Id)?.time ?? ulong.MinValue;
            ulong scheduledThisWeek =
                TimeKeeper.WeekStart + (ulong)((e.dayOfWeek.ToInt() + e.timeOfDay) * TimeKeeper.TicksPerInGameDay);
            ulong lastScheduled =
                scheduledThisWeek <= TimeKeeper.CurrentTick
                ? scheduledThisWeek
                : scheduledThisWeek - (ulong)(TimeKeeper.TicksPerInGameDay * WeekDayHelper.DaysInWeek);

            // If the last occurrence was after the last scheduled occurrence, then the
            // event has already occurred.
            if (lastOccurrence >= lastScheduled) continue;

            e.action();
            history.LogEvent(e.Id);
            if (debug)
            {
                Debug.Log($"Event occurred: {e.Id}");
                NotificationManager.Notify($"Event occurred: {e.Id}");
            }

        }
    }


    // Event functions -----------------------------------------------------------------

    private static void DoNaturalSpawns()
    {
        RegionInfo region = ContinentManager.CurrentRegion.info;
        if (region.naturalSpawns == null) return;

        foreach (RegionInfo.NaturalSpawnConfig spawn in region.naturalSpawns.Where(
            spawn => FindObjectsOfType<Actor>()
                    .Where(actor => !actor.GetData().Health.IsDead)
                    .Count(
                        actor => ContentLibrary.Instance.ActorTemplates.Get(spawn.actorTemplate)
                            .races.Contains(actor.GetData().RaceId))
                < spawn.maxActorCount))
            for (int i = 0; i < spawn.dailySpawnRolls; i++)
            {
                if (Random.value > spawn.dailySpawnProbability) continue;
                GenerateAndSpawn(spawn.actorTemplate);
            }
    }

    /// Generates the actor from the given template, registers it, and spawns it in the
    /// current region.
    private static Actor GenerateAndSpawn(string actorTemplate)
    {
        ActorData actor = ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get(actorTemplate));
        // Register the actor
        ActorRegistry.Register(actor);
        // Spawn the actor
        Vector2Int spawnPos = RegionMapManager.FindRegionEntranceTile(out Direction spawnDir);
        return ActorSpawner.Spawn(actor.ActorId, spawnPos, SceneObjectManager.WorldSceneId, spawnDir);
    }

    /// Refills all ShopStations in the current region, clearing their current contents.
    private static void RefillShopStations()
    {
        foreach (ShopStation shop in FindObjectsOfType<ShopStation>()) shop.RegenerateStock();
    }

    /// Spawns actors as new residents to fill any empty houses in the current region.
    private static void SpawnNewResidents()
    {
        // TODO for the love of god, move this somewhere else

        SettlementManager sm = FindObjectOfType<SettlementManager>();
        Debug.Assert(sm != null, "SettlementManager not found in scene.");
        if (sm == null) return;

        HashSet<string> availableWorkplaces = new(sm.GetAvailableWorkplaces(ContinentManager.CurrentRegionId));

        foreach (string buildingScene in sm.GetUnoccupiedBuildings(ContinentManager.CurrentRegionId))
        {
            BuildingInfo info = sm.GetBuildingInfo(buildingScene, ContinentManager.CurrentRegionId);
            Debug.Assert(info != null, "Building not found in scene.");

            if (info.type == BuildingInfo.Type.Workplace) continue;

            int numResidents = info.maxResidents;
            for (int i = 0; i < numResidents; i++)
            {
                string template = "homesteader";
                string workplace = null;
                string profession = null;

                if (i == 0 && info.type == BuildingInfo.Type.Hybrid && info.RequiredProfession != null)
                {
                    // The first actor spawned in a hybrid building is the worker.
                    workplace = buildingScene;
                    profession = info.RequiredProfession;
                }
                else if (info.type == BuildingInfo.Type.Dwelling && availableWorkplaces.Any())
                {
                    workplace = availableWorkplaces.PickRandom();
                    availableWorkplaces.Remove(workplace);
                    profession = sm.GetBuildingInfo(workplace, ContinentManager.CurrentRegionId).RequiredProfession;
                }

                // If there is an actor template for this profession, use it.
                if (profession != null && ContentLibrary.Instance.ActorTemplates.Contains(profession))
                    template = profession;

                Actor actor = GenerateAndSpawn(template);
                actor.GetData().Role = profession;

                sm.AddResident(actor.ActorId, ContinentManager.CurrentRegionId, buildingScene, workplace);
            }
        }
    }


    // Event classes -------------------------------------------------------------------

    /// An event that occurs periodically.
    private class PeriodicEvent
    {
        public readonly Action action;
        /// The number of in-game days between occurrences.
        public readonly float daysBetweenOccurrences;

        public PeriodicEvent(string id, Action action, float daysBetweenOccurrences)
        {
            Id = id;
            this.action = action;
            this.daysBetweenOccurrences = daysBetweenOccurrences;
        }

        public virtual string Id { get; }
    }

    /// An event whose ID is post-fixed with the id of the current region.
    private class PerRegionPeriodicEvent : PeriodicEvent
    {
        public PerRegionPeriodicEvent(string id, Action action, float daysBetweenOccurrences) : base(
            id,
            action,
            daysBetweenOccurrences) { }

        public override string Id => base.Id + "_" + ContinentManager.CurrentRegion.info.Id;
    }

    /// An event which occurs at a fixed time of the week.
    private class WeeklyEvent
    {
        public readonly Action action;
        public readonly WeekDay dayOfWeek;
        public readonly float timeOfDay;

        public WeeklyEvent(string id, Action action, WeekDay dayOfWeek, float timeOfDay)
        {
            Id = id;
            this.action = action;
            this.dayOfWeek = dayOfWeek;
            this.timeOfDay = timeOfDay;
        }

        public virtual string Id { get; }
    }

    /// An event which occurs at a fixed time of the week, and is post-fixed with the id
    /// of the current region.
    private class PerRegionWeeklyEvent : WeeklyEvent
    {
        public PerRegionWeeklyEvent(string id, Action action, WeekDay dayOfWeek, float timeOfDay) : base(
            id,
            action,
            dayOfWeek,
            timeOfDay) { }

        public override string Id => base.Id + "_" + ContinentManager.CurrentRegion.info.Id;
    }
}
