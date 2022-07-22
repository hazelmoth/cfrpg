namespace AI.Nodes
{
    public class MeleeFight : Node
    {
        // The maximum distance to the target before we start throwing punches
        private const float TargetDist = 1f;

        // Time to pause after punching
        private const float PauseDuration = 0.5f;

        // Max duration to run away after attacking
        private const float FleeTime = 4f;

        private readonly Actor agent;
        private readonly Actor target;
        private Node repeater;

        public MeleeFight(Actor agent, Actor target)
        {
            this.agent = agent;
            this.target = target;
        }

        protected override void OnCancel()
        {
            if (repeater != null && !repeater.Stopped) repeater.Cancel();
        }

        protected override void Init()
        {
            // Repeat the same attack sequence endlessly.
            repeater = new Repeater(
                () => new Sequencer(
                    () => new ImpatientRepeater(
                        () => new GoToActor(agent, target, TargetDist),
                        maxRestartTime: 1f,
                        finishOnSuccess: true),
                    () => new MeleeAttack(agent, target),
                    () => new Wait(PauseDuration),
                    () => new TimeLimit(() => new MoveRandomly(agent), FleeTime)));
        }

        protected override Status OnUpdate()
        {
            return repeater.Update();
        }
    }
}