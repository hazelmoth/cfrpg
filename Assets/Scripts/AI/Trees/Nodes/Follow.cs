using AI.Trees;
using AI.Trees.Nodes;

public class Follow : Node
{
    // Interval at which path is recalculated
    private const float RefreshTime = 0.5f;
    
    private readonly Actor agent;
    private readonly Actor target;
    private readonly float targetDist;
    private Node subNode;
    
    public Follow(Actor agent, Actor target, float targetDist)
    {
        this.agent = agent;
        this.target = target;
        this.targetDist = targetDist;
    }
    
    protected override void Init()
    {
        subNode = new ImpatientRepeater(
            new Task(
                typeof(GoToActor), 
                new object[] {agent, target, targetDist}
            ),
            RefreshTime
        );
    }

    protected override Status OnUpdate()
    {
        subNode.Update();
        return Status.Running;
    }
} 