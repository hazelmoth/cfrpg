using System;
using System.Collections;
using System.Collections.Generic;
using AI.Trees;
using AI.Trees.Nodes;
using UnityEngine;

namespace AI.Trees.Nodes
{
    public class MeleeFight : Node
    {
        // The maximum distance to the target before we start throwing punches
        private const float TargetDist = 1f;

        // Time to pause after punching
        private const float PauseDuration = 0.5f;
        
        // Max duration to run away after attacking
        private const float FleeTime = 4f;

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
            // Repeat the same attack sequence endlessly.
            repeater = new Repeater(
                new Task(
                    typeof(Sequencer),
                    new object[]
                    {
                        new List<Task>
                        {
                            new Task(
                                // Go to the target, recalculating path every second
                                typeof(ImpatientRepeater),
                                new object[] {
                                    new Task(
                                        typeof(GoToActor),
                                        new object[] {agent, target, TargetDist}
                                    ),
                                    1f,
                                    true
                                }
                            ),
                            new Task(
                                // Attack the target
                                typeof(MeleeAttack),
                                new object[] {agent, target}
                            ),
                            new Task(
                                // Pause for a moment after attacking
                                typeof(Wait),
                                new object[] {PauseDuration}
                            ),
                            new Task(
                                // Run away
                                typeof(TimeLimit),
                                new object[] {
                                    new Task(
                                        typeof(MoveRandomly),
                                        new object[] {agent}
                                    ),
                                    FleeTime
                                }
                            )
                        }
                    }
                )
            );
        }

        protected override Status OnUpdate()
        {
            return repeater.Update();
        }
    }
}
