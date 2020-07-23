using ActorComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for triggering 'random' events like traders arriving
public class Director : MonoBehaviour
{
    TimeKeeper.DateTime lastTraderArrival;

    // Update is called once per frame
    void Update()
    {
        if (TimeKeeper.daysBetween(lastTraderArrival, TimeKeeper.CurrentDateTime) > 1)
        {
            lastTraderArrival = TimeKeeper.CurrentDateTime;

            // Trigger a trader i guess?
            ActorData newTrader = ActorGenerator.Generate(ContentLibrary.Instance.CharacterGenTemplates.Get("trader"));
            newTrader.ActorComponents.Add(new Trader(newTrader.actorId));
            ActorRegistry.RegisterActor(newTrader);
            Vector2 spawn = ActorSpawnpointFinder.FindSpawnPoint(SceneObjectManager.WorldSceneId);
            ActorSpawner.Spawn(newTrader.actorId, spawn, SceneObjectManager.WorldSceneId);
            Debug.Log("New trader spawned.");
        }
    }
}
