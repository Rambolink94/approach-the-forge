using Godot;

namespace ApproachTheForge.Utility.Extensions;

public static class NodeExtensions
{
    public static T GetTypeFromChildren<T>(this Node node, int maxDepth = int.MaxValue, bool includeInternal = false, int currentDepth = 0)
        where T : Node
    {
        if (node is T foundNode)
        {
            return foundNode;
        }

        if (currentDepth >= maxDepth)
        {
            return null;
        }

        foreach (Node child in node.GetChildren(includeInternal))
        {
            var found = child.GetTypeFromChildren<T>(maxDepth, includeInternal, currentDepth);
            if (found is not null)
            {
                return found;
            }
        }

        // Had no children
        return null;
    }
}