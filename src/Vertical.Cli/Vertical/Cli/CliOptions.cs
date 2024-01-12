using Vertical.Cli.Binding;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

namespace Vertical.Cli;

public sealed class CliOptions
{
    /// <summary>
    /// Gets a collection of global value converters.
    /// </summary>
    public ICollection<ValueConverter> Converters { get; } = new List<ValueConverter>();

    /// <summary>
    /// Gets a collection of validators.
    /// </summary>
    public ICollection<Validator> Validators { get; } = new List<Validator>();

    /// <summary>
    /// Gets a collection of application defined model binders.
    /// </summary>
    public ICollection<ModelBinder> ModelBinders { get; } = new List<ModelBinder>();

    /// <summary>
    /// Gets whether to automatically display argument exceptions.
    /// </summary>
    public bool DisplayExceptions { get; set; } = true;

    /// <summary>
    /// Gets whether to throw binding exceptions.
    /// </summary>
    public bool ThrowExceptions { get; set; } = false;

    /// <summary>
    /// Gets a function that creates the help renderer at the moment it is needed.
    /// </summary>
    public Func<IHelpFormatter>? HelpFormatterFactory { get; set; }

    internal IHelpFormatter CreateHelpFormatter()
    {
        return HelpFormatterFactory?.Invoke()
               ?? new DefaultHelpFormatter(
                   new DefaultHelpProvider(),
                   Console.Out,
                   Console.WindowWidth);
    }
}