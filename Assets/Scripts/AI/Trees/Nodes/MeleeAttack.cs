using AI.Trees.Nodes;
using UnityEngine;

public class MeleeAttack : Node
{
    private Actor actor;
    private Vector2 direction;
    
    public MeleeAttack(Actor agent, Actor target)
    {
        this.actor = agent;
        direction = (target.transform.position - agent.transform.position).normalized;
    }
    
    public MeleeAttack(Actor actor, Vector2 direction)
    {
        this.actor = actor;
        this.direction = direction;
    }

    protected override void Init()
    {
        ActorAttackHandler attackHandler = actor.GetComponent<ActorAttackHandler>();
        Debug.Assert(attackHandler != null);

        attackHandler.ThrowPunch(direction);
    }

    protected override Status OnUpdate()
    {
        // This behaviour finishes in one frame
        return Status.Success;
    }
}