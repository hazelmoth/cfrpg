using ActorComponents;
using UnityEngine;
using SettlementSystem;
using System;

// Decides what the Actor should do.
public class ActorBehaviourAi : MonoBehaviour
{
	private Actor actor;
	private ActorPhysicalCondition actorCondition;
	private ActorBehaviourExecutor executor;
	private SettlementManager settlement;

    // Update is called once per frame
    private void Update()
    {
	    if (actor == null)
	    {
		    actor = GetComponent<Actor>();
			executor = GetComponent<ActorBehaviourExecutor>();
	    }

		if (settlement == null)
		{
			settlement = GameObject.FindObjectOfType<SettlementManager>();
			if (settlement == null)
			{
				Debug.LogError("SettlementManager object not found");
			}
		}
		if (actor.PlayerControlled)
	    {
		    return;
	    }

		executor.Execute(EvaluateBehaviour(out object[] args), args);
    }

	private Type EvaluateBehaviour (out object[] args)
	{
		if (actorCondition == null)
		{
			actorCondition = actor.GetData().PhysicalCondition;
		}
		if (executor == null)
		{
			executor = GetComponent<ActorBehaviourExecutor>();
		}

		args = new object[] { actor }; // All of these behaviours only take one parameter

		// Traders always trade
		if (actor.GetData().GetComponent<Trader>() != null)
		{
			return typeof(TraderBehaviour);
		}

		string faction = actor.GetData().FactionStatus.FactionId;
		if (!string.IsNullOrEmpty(faction) && faction == ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId)
		{
			return typeof(SettlerBehaviour);
		}

		// Apparently not a settler or a trader; just a nobody.
		// I guess he'll just walk around like a loser.
		return typeof(GoForWalkBehaviour);
	}
}
