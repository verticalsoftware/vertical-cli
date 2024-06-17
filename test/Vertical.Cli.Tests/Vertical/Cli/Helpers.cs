namespace Vertical.Cli;

public static class Unit
{
    public static T Create<T>(Func<T> factory) => factory();
}

public record Model<T>(T Value);