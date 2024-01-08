namespace Vertical.Cli.Conversion;

/// <summary>
/// Represents a <see cref="ValueConverter"/> that uses an underlying delegate to
/// perform the conversion activity.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
internal sealed class DelegateConverter<T> : ValueConverter<T> where T : notnull
{
    private readonly Func<ConversionContext<T>, T> _function;

    internal DelegateConverter(Func<string, T> function)
        : this(context => function(context.Value))
    {
    }

    internal DelegateConverter(Func<ConversionContext<T>, T> function)
    {
        _function = function;
    }
    
    /// <inheritdoc />
    public override T Convert(ConversionContext<T> context) => _function(context);
}