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
    FileInfo Root,
    bool NoCompile,
    bool NoSymbols)
{
    public string? ApiKey { get; set; }
    public string? Source { get; set; }
    public TimeSpan? Timeout { get; set; }
}

public class ParametersBinder : ModelBinder<Parameters>
{
    /// <inheritdoc />
    public override Parameters BindInstance(IBindingContext bindingContext)
    {
        return new Parameters(
            bindingContext.GetValue<FileInfo>("root"),
            bindingContext.GetValue<bool>("--no-compile"),
            bindingContext.GetValue<bool>("--no-symbols"))
        {
            ApiKey = bindingContext.GetValue<string>("ApiKey"),
            Source = bindingContext.GetValue<string>("Source"),
            Timeout = bindingContext.GetValue<TimeSpan?>("Timeout")
        };
    }
}

public readonly record struct Point(int X, int y);