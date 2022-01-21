using System.Collections.Generic;

namespace AI.Trees.Nodes
{
    /// A behaviour node that makes the actor wander around aimlessly. The actor will
    /// first go outside if they are in an interior.
    public class Wander : Node
    {
        private Node subNode;
        private Actor agent;

        public Wander(Actor agent)
        {
            this.agent = agent;
        }
        
        /// Returns a Task to create a Node of this type.
        public static Task MakeTask(Actor agent)
        {
            return new Task(typeof(Wander), new object[] {agent});
        }
        
        protected override void Init()
        {
            subNode = new Repeater(
                () => new Conditional(
                    () => agent.CurrentScene == SceneObjectManager.WorldSceneId,
                    () => new WanderLocal(agent),
                    () => new GoToScene(agent, SceneObjectManager.WorldSceneId)));
        }

        protected override Status OnUpdate()
        {
            return subNode.Update();
        }

        protected override void OnCancel()
        {
            if (subNode != null && !subNode.Stopped) subNode.Cancel();
        }
    }
}
