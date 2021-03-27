using System.Collections;
using System.Collections.Generic;
using AI.Trees;
using AI.Trees.Nodes;
using UnityEngine;

public class MeleeFight : Node
{
    // The maximum distance to the target before we start throwing punches
    private const float TargetDist = 1f;

    // Time to pause after punching
    private const float PauseDuration = 0.5f;
    
    private Actor agent;
    private Actor target;
    private Node repeater;

    public MeleeFight(Actor agent, Actor target)
    {
        this.agent = agent;
        this.target = target;
    }
    
    protected override void Init()
    {
        repeater = new Repeater(
            new Task(
                typeof(Sequencer),
                new object[] {new List<Task> {
                    new Task(
                        typeof(GoToActor),
                        new object[] { agent, target, TargetDist }
                    ),
                    new Task(
                        typeof(MeleeAttack),
                        new object[] { agent, target }
                    ),
                    new Task(
                        typeof(Wait),
                        new object[] { PauseDuration }
                    ),
                    new Task(
                        typeof(MoveRandomly),
                        new object[] { agent }
                    )
                }}
            )
        );
    }

    protected override Status OnUpdate()
    {
        return repeater.Update();
    }
}
