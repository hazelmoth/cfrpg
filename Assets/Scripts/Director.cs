using SettlementSystem;
using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

// Responsible for triggering 'random' events like traders arriving
public class Director : MonoBehaviour
{
    private const float DaysBetweenTraders = 1;

    private History history;
    private SettlementManager settlement;

    private void Start()
    {
        history = FindObjectOfType<History>();
        if (history == null)
        {
            Debug.LogError("Couldn't find History object in scene!");
        }

        settlement = FindObjectOfType<SettlementManager>();
        if (settlement == null)
        {
            Debug.LogError("Couldn't find SettlementManager object in scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameInitializer.InitializationFinished)
        {
            return;
        }

        ulong lastTraderArrival = 0;
        History.Event lastTraderEvent = history.GetMostRecent(History.TraderArrivalEvent);
        if (lastTraderEvent != null)
        {
            lastTraderArrival = lastTraderEvent.time;
        }

        if (TimeKeeper.DaysBetween(lastTraderArrival, TimeKeeper.CurrentTick) > DaysBetweenTraders)
        {
             TriggerTraderArrival();
        }

        foreach (House house in settlement.Houses)
        {
            if (house.Owner == null)
            {
                //TriggerNewSettlerArrival(house);
            }
        }
    }

    private void TriggerTraderArrival()
    {
        // Generate a new trader
        ActorData newTrader = ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get("trader"));
        newTrader.Profession = Professions.TraderProfessionID;
        // Register the actor
        ActorRegistry.Register(newTrader);
        // Give him some money
        newTrader.Wallet.SetBalance(Random.Range(100, 1000));
        // Give him seeds to sell
        newTrader.Inventory.AttemptAddItem(new ItemStack("wheat_seeds", Random.Range(1, 30)));
        // Spawn the actor
        Vector2Int spawn = RegionMapManager.FindWalkableEdgeTile(Direction.Left);
        ActorSpawner.Spawn(newTrader.ActorId, spawn, SceneObjectManager.WorldSceneId);

        // Do a notification
        NotificationManager.Notify(newTrader.ActorName + ", a trader, is stopping by!");
        // Log event
        history.LogEvent(History.TraderArrivalEvent);
    }

    private void TriggerNewSettlerArrival(House house)
    {
        ActorData newActor = ActorGenerator.Generate();
        // Register the actor
        ActorRegistry.Register(newActor);
        newActor.Wallet.SetBalance(Random.Range(100, 1000));

        // Add to the player's faction
        ActorData player = ActorRegistry.Get(PlayerController.PlayerActorId).data;
        if (player.FactionStatus.FactionId == null) player.FactionStatus.FactionId = FactionManager.CreateFaction(player.ActorId);
        newActor.FactionStatus.FactionId = player.FactionStatus.FactionId;

        // Spawn the actor
        Vector2Int spawn = RegionMapManager.FindWalkableEdgeTile(Direction.Up);
        ActorSpawner.Spawn(newActor.ActorId, spawn, SceneObjectManager.WorldSceneId);

        house.Owner = newActor.ActorId;

        // Do a notification
        NotificationManager.Notify(newActor.ActorName + " is moving into your settlement!");
        // Log event
        history.LogEvent(History.NewSettlerEvent);
    }
}
