using System.Collections;
using System.Collections.Generic;
using AI.Behaviours;
using AI.Trees.Nodes;

public class Settler : Node
{
    private const float SleepStart = 0.9f;
    private const float SleepEnd = 0.25f;
    
    private readonly Actor agent;
    private readonly Node child;
    
    public Settler(Actor agent)
    {
        this.agent = agent;
        
        child = new RestartingConditional(
            (() => TimeKeeper.TimeAsFraction > SleepStart || TimeKeeper.TimeAsFraction < SleepEnd),
            (() => new BehaviourNode(new SleepBehaviour(agent))),
            (() => new Wander(agent))
        );
    }

    protected override void Init() { }

    protected override void OnCancel() => child.Cancel();

    protected override Status OnUpdate() => child.Update();
}
