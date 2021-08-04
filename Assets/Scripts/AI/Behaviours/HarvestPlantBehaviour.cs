using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;

namespace AI.Behaviours
{
    public class HarvestPlantBehaviour : IAiBehaviour
    {
        private readonly Actor actor;
        private readonly ActorBehaviourExecutor.ExecutionCallbackFailable callback;
        private Coroutine harvestCoroutine;
        private readonly HarvestablePlant targetPlant;

        public HarvestPlantBehaviour(
            Actor Actor,
            HarvestablePlant targetPlant,
            ActorBehaviourExecutor.ExecutionCallbackFailable callback)
        {
            this.actor = Actor;
            this.targetPlant = targetPlant;
            this.callback = callback;
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }

        public void Cancel()
        {
            if (harvestCoroutine != null)
                actor.StopCoroutine(harvestCoroutine);
            IsRunning = false;
            callback(false);
        }

        public void Execute()
        {
            actor.StartCoroutine(HarvestPlantCoroutine());
            IsRunning = true;
        }

        private IEnumerator HarvestPlantCoroutine()
        {
            ImmutableList<DroppedItem> items = null;
            if (targetPlant != null) targetPlant.Harvest(out items);

            // Wait a bit before picking up the item
            yield return new WaitForSeconds(0.5f);

            foreach (DroppedItem item in items.Where(
                item => item != null && actor.GetData().Inventory.AttemptAddItem(item.Item)))
                Object.Destroy(item.gameObject);

            callback?.Invoke(true);
            IsRunning = false;
        }
    }
}