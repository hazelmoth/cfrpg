using UnityEngine;

namespace AI.Trees.Nodes
{
    /**
     * <summary>
     *     A Node represents an execution of a single behaviour tree or sub-tree, which
     *     may or may not continue over multiple frames. <br />
     * </summary>
     * <remarks>
     *     Nodes are run by calling Update() once per frame, which they may propagate
     *     to any sub-trees contained within. Update() returns a Node.Status, which
     *     communicates whether the node's task completed successfully or unsuccessfully,
     *     or is still running. <br />
     * </remarks>
     * <remarks>
     *     If a Node returns Status.Running it is expected that the Node is updated again
     *     in the next frame, unless the client or parent Node chooses to cancel that
     *     Node prematurely--in which case it is expected to call Cancel(). Nodes
     *     should not cause an invalid state if they are cancelled at any time. <br />
     * </remarks>
     * <remarks>
     *     In general Update() should not be called after a Node has returned a status
     *     of Success or Failure. If a node needs to be run multiple times, the node or
     *     class containing it should handle this by replacing it with a new node of the
     *     same type. <br />
     * </remarks>
     * <remarks>
     *     Note that Nodes need not ever return a Status of Success or Failure; it is
     *     valid for a node to only conclude by cancellation.
     * </remarks>
     */
    public abstract class Node
    {
        public enum Status
        {
            Success, // The behaviour has finished successfully
            Failure, // The behaviour has finished unsuccessfully
            Running // The behaviour is still running.
        }

        public bool Started { get; private set; }

        public bool Stopped { get; private set; }

        /// Runs this tree for a single frame, and returns its current state.
        public Status Update()
        {
            if (Stopped)
            {
                Debug.LogWarning("Tried to update a stopped node.");
                return Status.Failure;
            }

            if (!Started) Init();
            Started = true;

            var result = OnUpdate();
            if (result != Status.Running) Stopped = true;
            return result;
        }

        /// Cancel must be called by a client if they choose to stop updating
        /// a node before it returns Success or Failure.
        public void Cancel()
        {
            if (Stopped)
            {
                Debug.LogWarning("Tried to cancel an already stopped node.");
                return;
            }

            Stopped = true;
            OnCancel();
        }

        /// Init is called before the first Update.
        protected abstract void Init();

        protected abstract void OnCancel();

        protected abstract Status OnUpdate();
    }
}