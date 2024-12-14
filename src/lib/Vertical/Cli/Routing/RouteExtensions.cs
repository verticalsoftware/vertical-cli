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
        return
            ancestor.Pattern.Length < path.Pattern.Length &&
            path.Pattern.StartsWith(ancestor.Pattern);
    }

    /// <summary>
    /// Determines if this route is a child of the given instance.
    /// </summary>
    /// <param name="path">This instance</param>
    /// <param name="ancestor">The route used to determine lineage</param>
    /// <returns><c>bool</c></returns>
    public static bool IsChildOf(this RoutePath path, RoutePath ancestor) => ancestor.Equals(path.GetParentPath());

    /// <summary>
    /// Gets the parent path.
    /// </summary>
    /// <param name="path">The path</param>
    /// <returns><see cref="RoutePath"/> or <c>null</c> if the specified instance is a root path.</returns>
    public static RoutePath? GetParentPath(this RoutePath path)
    {
        var spaceIndex = path.Pattern.LastIndexOf(' ');

        return spaceIndex == -1 ? null : new RoutePath(path.Pattern[..spaceIndex]);
    }

    /// <summary>
    /// Gets the descendant path (parent path removed).
    /// </summary>
    /// <param name="path">Parent path</param>
    /// <param name="descendant">Descendant path</param>
    /// <returns></returns>
    public static string GetDescendantPath(this RoutePath path, RoutePath descendant)
    {
        return descendant.Pattern[(path.Pattern.Length+1)..];
    }
}