using System;
using ContinentMaps;
using MyBox;
using SettlementSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AI.Trees.Nodes
{
    /// A node that has an agent go their to workplace in the current region. If they
    /// have no workplace, they will just wander around.
    public class SettlerWork : Node
    {
        private Actor agent;
        private SettlementManager sm;
        private string workplaceScene;
        private Node subNode;

        public SettlerWork(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            sm = Object.FindObjectOfType<SettlementManager>();
            if (sm == null)
            {
                Debug.LogError("No settlement manager found!");
                subNode = new Wander(agent);
                return;
            }

            // TODO we should have an intermediate case;
            // if there is a workplace but no station, we should wander inside workplace
            subNode = new Repeater(
                () => new Conditional(
                    () => GetWorkstation() != null,
                    () => new GoToAndWorkAtStation(agent, GetWorkstation()),
                    () => new Wander(agent)));
        }

        protected override void OnCancel()
        {
            if (subNode is { Stopped: false }) subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            return subNode.Update();
        }

        private string GetWorkplaceScene()
        {
            return sm.GetWorkplaceScene(agent.ActorId, ContinentManager.CurrentRegionId);
        }

        /// Returns the type of workstation that the agent uses for their job, or null
        /// if the agent doesn't use a workstation.
        private Type GetWorkstationType()
        {
            string actorRole = agent.GetData().Role;
            if (actorRole.IsNullOrEmpty()) return null;

            return actorRole switch
            {
                Roles.Shopkeeper => typeof(ShopStation),
                Roles.Banker => typeof(BankDesk),
                Roles.Sheriff => typeof(SheriffDesk),
                _ => null
            };
        }

        private NonPlayerWorkstation GetWorkstation()
        {
            if (GetWorkstationType() == null) return null;
            if (GetWorkplaceScene() == null) return null;
            GameObject workplaceSceneRoot = SceneObjectManager.GetSceneObjectFromId(GetWorkplaceScene());
            if (workplaceSceneRoot == null) return null;

            return workplaceSceneRoot.GetComponentInChildren(GetWorkstationType()) as NonPlayerWorkstation;
        }
    }
}
