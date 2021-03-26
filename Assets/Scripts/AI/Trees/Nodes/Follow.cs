using AI.Trees.Nodes;

public class Follow : Node
{
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
        subNode = new Repeater(typeof(GoToActor), new object[] {agent, target, targetDist});
    }

    protected override Status OnUpdate()
    {
        subNode.Update();
        return Status.Running;
    }
} 