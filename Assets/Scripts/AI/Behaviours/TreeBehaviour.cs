using System.Collections;
using AI.Trees.Nodes;
using UnityEngine;

// A Behaviour which executes a behaviour tree (Node)
namespace AI.Behaviours
{
    public class TreeBehaviour : AIBehaviour
    {
        private Node tree;
        private Coroutine coroutine;
    
        public TreeBehaviour(Actor actor, Node tree) : base(actor)
        {
            Debug.Log("Constructing TreeBehaviour.");
            this.tree = tree;
        }

        protected override void OnExecute()
        {
            coroutine = GlobalCoroutineObject.Instance.StartCoroutine(Coroutine());
        }

        protected override void OnCancel()
        {
            if (coroutine != null)
            {
                GlobalCoroutineObject.Instance.StopCoroutine(coroutine);
            }
        }

        private IEnumerator Coroutine()
        {
            while (tree.Update() == Node.Status.Running)
            {
                Debug.Log("TreeBehaviour still running.");
                yield return null;
            }

            Debug.Log("TreeBehaviour finished.");
            IsRunning = false;
        }
    }
}
