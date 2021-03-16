using UnityEngine;
using SettlementSystem;
using System;
using Behaviours;

// Decides what the Actor should do.
public class ActorBehaviourAi : MonoBehaviour
{
	private Actor actor;
	private ActorBehaviourExecutor executor;
	private SettlementManager settlement;

    // Update is called once per frame
    private void Update()
    {
	    if (actor == null)
	    {
		    actor = GetComponent<Actor>();
			executor = GetComponent<ActorBehaviourExecutor>();
			Debug.Assert(actor != null);
			Debug.Assert(executor != null);
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
			executor.ForceCancelBehaviours();
		    return;
	    }

		executor.Execute(EvaluateBehaviour(actor, out var args), args);
    }

	private static Type EvaluateBehaviour (Actor actor, out object[] args)
	{
		Debug.Assert(!actor.PlayerControlled, "Tried to evaluate AI for player actor!");
		
		args = new object[] { actor }; // All of these behaviours only take one parameter
		
		// Dead people don't do much
		if (actor.GetData().PhysicalCondition.IsDead)
		{
			return typeof(BeDeadBehaviour);
		}

		// Traders always trade
		if (actor.GetData().Profession == Professions.TraderProfessionID)
		{
			return typeof(TraderBehaviour);
		}

		// Same faction as the player = this is a settler!
		string faction = actor.GetData().FactionStatus.FactionId;
		if (faction != null && faction == ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId)
		{
			return typeof(SettlerBehaviour);
		}

		// No faction = violent drifter.
		if (faction == null)
		{
			return typeof(DrifterBehaviour);
		}

		// Apparently not a settler or a trader; just a nobody.
		// I guess he'll just walk around like a loser.
		Debug.LogWarning("Couldn't determine a good behaviour tree for this actor: " + actor.ActorId);
		return typeof(GoForWalkBehaviour);
	}
}
