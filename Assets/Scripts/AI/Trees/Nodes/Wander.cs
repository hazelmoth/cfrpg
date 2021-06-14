using System.Collections.Generic;

namespace AI.Trees.Nodes
{
    // A behaviour node that makes the actor wander around aimlessly.
    public class Wander : Node
    {
        private Node subNode;
        private Actor agent;

        public Wander(Actor agent)
        {
            this.agent = agent;
        }
        
        // Returns a Task to create a Node of this type.
        public static Task MakeTask(Actor agent)
        {
            return new Task(typeof(Wander), new object[] {agent});
        }
        
        protected override void Init()
        {
            subNode = new ImpatientRepeater(
                new Task(
                    typeof(Sequencer),
                    new object[] {
                        new List<Task> {
                            new Task(
                                typeof(MoveRandomly),
                                new object[] {agent, 20}
                            ),
                            new Task(
                                typeof(Wait),
                                new object[] {1.4f}
                            )
                        }
                    }
                ),
                maxRestartTime: 15f
            );
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