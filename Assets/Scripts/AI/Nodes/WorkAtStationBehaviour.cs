using UnityEngine;

namespace AI.Nodes
{
    /// A behaviour that makes an actor attempt to go to a station of a certain type and
    /// occupy it. If no station of the given type is available, the actor will just
    /// wander around instead.
    public class WorkAtStationBehaviour<T> : Node where T : NonPlayerWorkstation
    {
        private readonly Actor agent;
        private NonPlayerWorkstation station;
        private Node subNode;

        public WorkAtStationBehaviour(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            subNode = new Repeater(
                () => new Conditional(
                    () => station != null,
                    () => new GoToAndWorkAtStation(agent, station),
                    () => new Wander(agent)));
        }

        protected override void OnCancel()
        {
            subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            // so this isn't the best for performance TODO
            // also, won't work if there is more than one TODO
            if (station == null)
                station = Object.FindObjectOfType<T>();

            // Can't use it if it's in use by someone else
            if (station != null && station.Occupied && station.CurrentOccupier != agent) station = null;

            return subNode.Update();
        }
    }
}
