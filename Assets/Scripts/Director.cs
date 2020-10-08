using ActorComponents;
using UnityEngine;

// Responsible for triggering 'random' events like traders arriving
public class Director : MonoBehaviour
{
    private const float DaysBetweenTraders = 1;

    private History history;

    private void Start()
    {
        history = FindObjectOfType<History>();
        if (history == null)
        {
            Debug.LogError("Couldn't find History object in scene!");
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

        if (TimeKeeper.daysBetween(lastTraderArrival, TimeKeeper.CurrentTick) > DaysBetweenTraders)
        {
             TriggerTraderArrival();
        }
    }

    private void TriggerTraderArrival()
    {
        // Generate a new trader
        ActorData newTrader = ActorGenerator.Generate(ContentLibrary.Instance.CharacterGenTemplates.Get("trader"));
        newTrader.ActorComponents.Add(new Trader(newTrader.actorId));
        // Register the actor
        ActorRegistry.RegisterActor(newTrader);
        // Give him some money
        newTrader.Wallet.SetBalance(Random.Range(100, 1000));
        // Give him seeds to sell
        newTrader.Inventory.AttemptAddItem(new ItemStack("wheat_seeds", Random.Range(1, 30)));
        // Spawn the actor
        Vector2Int spawn = WorldMapManager.FindWalkableEdgeTile(Direction.Left);
        ActorSpawner.Spawn(newTrader.actorId, spawn, SceneObjectManager.WorldSceneId);

        // Do a notification
        NotificationManager.Notify(newTrader.ActorName + ", a trader, is stopping by!");
        // Log event
        history.LogEvent(History.TraderArrivalEvent);
    }
}
