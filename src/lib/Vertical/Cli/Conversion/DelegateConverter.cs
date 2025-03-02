namespace Vertical.Cli.Conversion;

internal sealed class DelegateConverter<T>(Func<string, T> function) : ValueConverter<T>
{
    /// <inheritdoc />
    public override T Convert(string str) => function(str);
}