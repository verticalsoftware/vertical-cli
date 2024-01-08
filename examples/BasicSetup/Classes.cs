// using Vertical.Cli.Conversion;
//
// namespace BasicSetup;
//
// public class Converter<T> : ValueConverter<T> where T : notnull
// {
//     /// <inheritdoc />
//     public override T Convert(ConversionContext<T> context)
//     {
//         throw new NotImplementedException();
//     }
// }
//

using Vertical.Cli.Binding;

public record Parameters(
    string Root,
    bool NoCompile,
    bool NoSymbols)
{
    public string? ApiKey { get; set; }
    public string? Source { get; set; }
    public TimeSpan? Timeout { get; set; }
}