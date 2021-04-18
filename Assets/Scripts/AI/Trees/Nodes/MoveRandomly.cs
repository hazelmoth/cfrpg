using UnityEngine;

namespace AI.Trees.Nodes
{
	// A Behaviour Node which moves the actor to a random nearby location and then finishes.
	public class MoveRandomly : Node
	{
		private readonly Actor actor;
		private readonly int stepsToWalk; // The number of random walk steps; not
										  // necessarily the actual # of steps.
		
		private Location destination;
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
			destination = new Location(destVector + new Vector2(0.5f, 0.5f), actor.CurrentScene);
			navSubBehaviour = new GoTo(actor, destination, 0.5f);
		}

		protected override Status OnUpdate()
		{
			return navSubBehaviour.Update();
		}
	}
}