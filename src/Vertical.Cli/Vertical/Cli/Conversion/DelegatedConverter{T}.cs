namespace Vertical.Cli.Conversion;

/// <summary>
/// Represents a <see cref="ValueConverter"/> that uses an underlying delegate to
/// perform the conversion activity.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
internal sealed class DelegatedConverter<T> : ValueConverter<T>
{
    private readonly Func<ConversionContext<T>, T> _function;

    internal DelegatedConverter(Func<string, T> function)
        : this(context => function(context.Value))
    {
    }

    internal DelegatedConverter(Func<ConversionContext<T>, T> function)
    {
        _function = function;
    }
    
    /// <inheritdoc />
    public override T Convert(ConversionContext<T> context) => _function(context);
}