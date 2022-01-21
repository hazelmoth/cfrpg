using AI.Trees.Nodes;
using ContinentMaps;
using SettlementSystem;
using UnityEngine;

/// This behaviour encapsulates the routine of actors who live as settlers in a region.
/// They will sleep in their house at night if they have one, and will either wander
/// around or go to work if they have a job.
public class SettlerBehaviour : Node
{
    private const float SleepStart = 0.9f;
    private const float SleepEnd = 0.25f;
    
    private readonly Actor agent;
    private SettlementManager sm;
    private Node child;
    
    public SettlerBehaviour(Actor agent)
    {
        this.agent = agent;
    }

    protected override void Init()
    {
        sm = Object.FindObjectOfType<SettlementManager>();
        if (sm == null) Debug.LogError("Could not find SettlementManager");

        child = new Conditional(
            () => TimeKeeper.TimeOfDay is > SleepStart or < SleepEnd,
            () => new GoToSleepBehaviour(agent),
            () => new Conditional(
                () => sm.GetWorkplaceScene(agent.ActorId, ContinentManager.CurrentRegionId) != null,
                () => new SettlerWork(agent),
                () => new Wander(agent)));
    }

    protected override void OnCancel() => child.Cancel();

    protected override Status OnUpdate() => child.Update();
}
