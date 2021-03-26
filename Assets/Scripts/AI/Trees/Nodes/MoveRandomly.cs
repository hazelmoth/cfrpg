using AI.Behaviours;
using UnityEngine;

namespace AI.Trees.Nodes
{
	// A Behaviour Node which moves the actor to a random nearby location and then finishes.
	public class MoveRandomly : Node
	{
		private readonly Actor actor;
		private readonly int stepsToWalk; // The number of random walk steps; not
										  // necessarily the actual # of steps.
		
		private TileLocation destination;
		private Node navSubBehaviour;

		public MoveRandomly(Actor actor, int stepsToWalk = 20)
		{
			this.actor = actor;
			this.stepsToWalk = stepsToWalk;
		}

		protected override void Init()
		{
			Vector2 destVector = Pathfinder.FindRandomNearbyPathTile(
				TilemapInterface.WorldPosToScenePos(actor.transform.position, actor.CurrentScene), 
				stepsToWalk,
				actor.CurrentScene);
			destination = new TileLocation(destVector.ToVector2Int(), actor.CurrentScene);
			navSubBehaviour = new BehaviourNode(new NavigateBehaviour(actor, destination, null));
		}

		protected override Status OnUpdate()
		{
			return navSubBehaviour.Update();
		}
	}
}