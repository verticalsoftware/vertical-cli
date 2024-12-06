namespace Vertical.Cli.Routing;

/// <summary>
/// Extensions to route types.
/// </summary>
public static class RouteExtensions
{
    /// <summary>
    /// Determines if this route is a descendant of the given instance.
    /// </summary>
    /// <param name="path">This instance</param>
    /// <param name="ancestor">The route used to determine lineage</param>
    /// <returns><c>bool</c></returns>
    public static bool IsDescendantOf(this RoutePath path, RoutePath ancestor)
    {
        return !ReferenceEquals(path, ancestor) && GetHierarchyDistance(ancestor.Pattern, path.Pattern) > 0;
    }

    /// <summary>
    /// Determines if this route is a child of the given instance.
    /// </summary>
    /// <param name="path">This instance</param>
    /// <param name="ancestor">The route used to determine lineage</param>
    /// <returns><c>bool</c></returns>
    public static bool IsChildOf(this RoutePath path, RoutePath ancestor)
    {
        return !ReferenceEquals(path, ancestor) && GetHierarchyDistance(ancestor.Pattern, path.Pattern) == 1;
    }

    /// <summary>
    /// Gets the descendant path (parent path removed).
    /// </summary>
    /// <param name="path">Parent path</param>
    /// <param name="descendant">Descendant path</param>
    /// <returns></returns>
    public static string GetDescendantPath(this RoutePath path, RoutePath descendant)
    {
        return descendant.Pattern[path.Pattern.Length..];
    }

    private static int GetHierarchyDistance(string parent, string child)
    {
        // Find where child intersects with parent
        var pos = 0;
        var len = Math.Min(parent.Length, child.Length);

        while (pos < len)
        {
            if (parent[pos] != child[pos])
                break;
            ++pos;
        }

        if (pos == 0)
            return -1;

        var spaceCount = 0;
        
        while (pos < child.Length)
        {
            if (child[pos] == ' ')
                ++spaceCount;

            ++pos;
        }

        return spaceCount;
    }
}