using Shouldly;

namespace Vertical.Cli.Routing;

public class RouteExtensionsTests
{
    private class ChildOfData : TheoryData<string, string, bool>
    {
        public ChildOfData()
        {
            Add("dz5", "dz5", false);
            Add("dz5", "dz5 create", true);
            Add("dz5", "dz5 create append", false);
        }
    }
    
    private class DescendantOfData : TheoryData<string, string, bool>
    {
        public DescendantOfData()
        {
            Add("dz5", "dz5", false);
            Add("dz5", "dz5 create", true);
            Add("dz5", "dz5 create append", true);
        }
    }

    [Fact]
    public void GetParentPath_Returns_Expected()
    {
        new RoutePath("dz5").GetParentPath().ShouldBeNull();
        new RoutePath("dz5 create").GetParentPath()!.Pattern.ShouldBe("dz5");
        new RoutePath("dz5 create new").GetParentPath()!.Pattern.ShouldBe("dz5 create");
    }
    
    [Theory]
    [ClassData(typeof(ChildOfData))]
    public void IsChildOf_Returns_Expected(string parent, string child, bool expected)
    {
        var (parentPath, childPath) = (new RoutePath(parent), new RoutePath(child));
        childPath.IsChildOf(parentPath).ShouldBe(expected);
    }
    
    [Theory]
    [ClassData(typeof(DescendantOfData))]
    public void IsDescendantOf_Returns_Expected(string parent, string child, bool expected)
    {
        var (parentPath, childPath) = (new RoutePath(parent), new RoutePath(child));
        childPath.IsDescendantOf(parentPath).ShouldBe(expected);
    }
}