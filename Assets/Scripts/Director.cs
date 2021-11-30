using System;
using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using ContinentMaps;
using JetBrains.Annotations;
using Popcron.Console;
using UnityEngine;
using Random = UnityEngine.Random;

/// Responsible for triggering 'random' events like traders arriving
public class Director : MonoBehaviour
{
    [UsedImplicitly]
    [Command("debugevents")]
    public static bool debug = false;
    /// A list of functions, each of which returns an event that is set to occur.
    private List<Func<ScheduledEvent>> events;

    private History history;

    private void Start()
    {
        // Event list --------------------------------------------------------------------

        events = new List<Func<ScheduledEvent>>
        {
            () => new PerRegionScheduledEvent("natural_spawns", DoNaturalSpawns, 1f),
        };

        history = FindObjectOfType<History>();
        if (history == null) Debug.LogError("Couldn't find History object in scene!");
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameInitializer.InitializationFinished) return;

        // Check if any events should occur
        foreach (ScheduledEvent e in events.Select(eventSupplier => eventSupplier.Invoke()))
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
    }

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
    private static void GenerateAndSpawn(string actorTemplate)
    {
        ActorData actor = ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get(actorTemplate));
        // Register the actor
        ActorRegistry.Register(actor);
        // Spawn the actor
        Vector2Int spawnPos = RegionMapManager.FindRegionEntranceTile(out Direction spawnDir);
        ActorSpawner.Spawn(actor.ActorId, spawnPos, SceneObjectManager.WorldSceneId, spawnDir);
    }

    private class ScheduledEvent
    {
        public readonly Action action;
        /// The number of in-game days between occurrences.
        public readonly float daysBetweenOccurrences;

        public ScheduledEvent(string id, Action action, float daysBetweenOccurrences)
        {
            Id = id;
            this.action = action;
            this.daysBetweenOccurrences = daysBetweenOccurrences;
        }

        public virtual string Id { get; }
    }

    /// An event whose ID is post-fixed with the id of the current region.
    private class PerRegionScheduledEvent : ScheduledEvent
    {
        public PerRegionScheduledEvent(string id, Action action, float daysBetweenOccurrences) : base(
            id,
            action,
            daysBetweenOccurrences) { }

        public override string Id => base.Id + "_" + ContinentManager.CurrentRegion.info.Id;
    }
}
