
/*
 * A Node represents an execution of a single behaviour tree or sub-tree, which
 * may or may not continue over multiple frames.
 *
 * Nodes are run by calling Update() once per frame, which they may propagate
 * to any sub-trees contained within. Update() returns a Node.Status, which
 * communicates whether the node's task completed successfully or unsuccessfully,
 * or is still running.
 *
 * If a Node returns Status.Running it is expected that the Node is updated again
 * in the next frame, unless the client or parent Node chooses to cancel that
 * Node prematurely--in which case it is expected to call Cancel(). Nodes
 * should not cause an invalid state if they are cancelled at any time.
 *
 * In general Update() should not be called after a Node has returned a status
 * of Success or Failure. If a node needs to be run multiple times, the node or
 * class containing it should handle this by replacing it with a new node of the
 * same type.
 *
 * Note that Nodes need not ever return a Status of Success or Failure; it is
 * valid for a node to only conclude by cancellation.
*/

using UnityEngine;

namespace AI.Trees.Nodes
{
    public abstract class Node
    {
        private bool hasInitialized = false;
        private bool stopped = false;

        public bool Started => hasInitialized;
        public bool Stopped => stopped;

        // Runs this tree for a single frame, and returns its current state.
        public Status Update()
        {
            if (stopped)
            {
                Debug.LogWarning("Tried to update a stopped node.");
                return Status.Failure;
            }
            if (!hasInitialized) Init();
            hasInitialized = true;
            
            Status result = OnUpdate();
            if (result != Status.Running) stopped = true;
            return result;
        }

        // Cancel must be called by a client if they choose to stop updating
        // a node before it returns Success or Failure.
        public void Cancel()
        {
            if (stopped)
            {
                Debug.LogWarning("Tried to cancel an already stopped node.");
                return;
            }
            stopped = true;
            OnCancel();
        }

        // Init is called before the first Update
        protected abstract void Init();
        
        protected abstract void OnCancel();

        protected abstract Status OnUpdate();

        public enum Status
        {
            Success, // The behaviour has finished successfully
            Failure, // The behaviour has finished unsuccessfully
            Running  // The behaviour is still running.
        }
    }
}
