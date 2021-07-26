using AI.Trees;
using AI.Trees.Nodes;
using SettlementSystem;
using UnityEngine;

// Decides what the Actor should do.
namespace AI
{
	public class ActorBehaviourAi : MonoBehaviour
	{
		private Actor actor;
		private ActorBehaviourExecutor executor;
		private SettlementManager settlement;

		// Update is called once per frame
		private void Update()
		{
			if (PauseManager.Paused) return;
			
			if (actor == null)
			{
				actor = GetComponent<Actor>();
				executor = GetComponent<ActorBehaviourExecutor>();
				Debug.Assert(actor != null);
				Debug.Assert(executor != null);
			}

			if (settlement == null)
			{
				settlement = FindObjectOfType<SettlementManager>();
				if (settlement == null)
				{
					Debug.LogError("SettlementManager object not found");
				}
			}
			if (actor.PlayerControlled)
			{
				executor.CancelTasks();
				return;
			}

			Task behaviour = EvaluateBehaviour(actor);
			executor.Execute(behaviour);
		}

		private Task EvaluateBehaviour (Actor actor)
		{
			Debug.Assert(!actor.PlayerControlled, "Tried to evaluate AI for player actor!");

			// Dead people don't do much
			if (actor.GetData().PhysicalCondition.IsDead)
			{
				return new Task(typeof(Wait), new object[] {1});
			}
			
			// Slug people are aggressive
			if (actor.GetData().RaceId == "slug_person")
			{
				return new Task(typeof(AggroAnimalBehaviour), new object[] {actor});
			}

			// If we're fighting someone, attack them.
			if (actor.HostileTargets.Count > 0)
			{
				return new Task(typeof(MeleeFight), new object[] {actor, actor.HostileTargets.Peek()});
			}

			// Traders always trade
			if (actor.GetData().Profession == Professions.TraderProfessionID)
			{
				// TODO: rewrite TraderBehaviour as behaviour tree
				//return typeof(TraderBehaviour);
			}

			// If the actor has a house in this region, they'll act as a settler.
			if (settlement.GetHouse(actor.ActorId) != null)
			{
				return new Task(typeof(Settler), new object[] {actor});
			}

			// Same faction as the player = this is a settler!
			string faction = actor.GetData().FactionStatus.FactionId;
			if (faction != null && faction == ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId)
			{
				// TODO: rewrite SettlerBehaviour as behaviour tree?
				// return typeof(SettlerBehaviour);
			}

			// No faction; probably wildlife. Wander around.
			if (faction == null)
			{
				return Wander.MakeTask(actor);
			}
			
			Debug.LogWarning("Couldn't determine a good behaviour tree for this actor: " + actor.ActorId);
			return new Task(typeof(Wait), new object[] {1});
		}
	}
}
