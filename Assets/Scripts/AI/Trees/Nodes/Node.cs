
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
 * Node prematurely--in which case it just won't call Update() any more. Nodes
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
namespace AI.Trees.Nodes
{
    public abstract class Node
    {
        private bool hasInitialized = false;
        
        // Runs this tree for a single frame, and returns its current state.
        public Status Update()
        {
            if (!hasInitialized) Init();
            hasInitialized = true;
            return OnUpdate();
        }

        // Init is called before the first Update
        protected abstract void Init();

        protected abstract Status OnUpdate();

        public enum Status
        {
            Success, // The behaviour has finished successfully
            Failure, // The behaviour has finished unsuccessfully
            Running  // The behaviour is still running.
        }
    }
}
