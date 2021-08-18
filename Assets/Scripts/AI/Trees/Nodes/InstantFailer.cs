namespace AI.Trees.Nodes
{
    /// A Node which does nothing but fail instantly.
    public class InstantFailer : Node
    {
        protected override void Init() { }

        protected override void OnCancel() { }

        protected override Status OnUpdate() => Status.Failure;
    }
}
