#if NET7_0_OR_GREATER
namespace Vertical.Cli.Conversion;

internal sealed class ParsableConverter<T> : ValueConverter<T> where T : IParsable<T>
{
    /// <inheritdoc />
    public override T Convert(ConversionContext<T> context)
    {
        return T.Parse(context.Value, provider: null);
    }
}
#endif