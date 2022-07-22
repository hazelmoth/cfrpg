using UnityEngine;

namespace AI.Nodes
{
    /// A Behaviour which will have an AI go to a specific workstation and occupy it.
    /// Always returns Status.Running unless the workstation is destroyed or nonexistent,
    /// in which case Status.Failure is returned.
    public class GoToAndWorkAtStation : Node
    {
        private const float WorkPositionAcceptableMargin = 0.1f;
        private readonly Actor agent;
        private NonPlayerWorkstation workstation;
        private Node subNode;

        public GoToAndWorkAtStation(Actor agent, NonPlayerWorkstation station)
        {
            workstation = station;
            this.agent = agent;
            if (workstation == null) Debug.LogError("Target workstation is null.");
        }

        protected override void Init()
        {
            subNode = new Repeater(
                () => new Sequencer(
                    () => new GoTo(agent, workstation.UserLocation, WorkPositionAcceptableMargin),
                    () => new Look(agent, workstation.UserDirection),
                    () => new Conditional(
                        CheckDistance,
                        () => new OccupyOccupiable(agent, workstation),
                        () => new InstantFailer())));
        }

        protected override void OnCancel()
        {
            subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            if (workstation == null) return Status.Failure;
            return subNode.Update();
        }

        private bool CheckDistance()
        {
            if (workstation == null) return false;

            float distance = Vector2.Distance(agent.Location.Vector2, workstation.UserLocation.Vector2);
            return distance <= WorkPositionAcceptableMargin;
        }
    }
}
