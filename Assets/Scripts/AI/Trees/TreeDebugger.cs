
using System.Reflection;
using System.Text;
using AI.Trees.Nodes;

namespace AI.Trees
{
    /// Tools for debugging behavior trees
    public static class TreeDebugger
    {
        /// Returns a string representation of the tree starting from the given node,
        /// indenting each level
        public static string DebugTree(Node root)
        {
            return DebugTree(root, 0);
        }

        private static string DebugTree(Node root, int indent)
        {
            StringBuilder sb = new();
            for (int i = 0; i < indent; i++) sb.Append("  ");

            if (root == null)
            {
                sb.AppendLine("null");
                return sb.ToString();
            }

            sb.AppendLine(root.GetType().Name
                + (root.Stopped ? " (stopped)" : "")
                + (!root.Started ? " (not started)" : ""));

            // Use reflection to find any private fields of type Node
            FieldInfo[] fields = root.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(Node))
                {
                    sb.Append(DebugTree(field.GetValue(root) as Node, indent + 1));
                }
            }

            return sb.ToString();
        }
    }
}
