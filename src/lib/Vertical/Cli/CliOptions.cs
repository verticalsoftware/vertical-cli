using Vertical.Cli.Conversion;
using Vertical.Cli.Validation;

namespace Vertical.Cli;

public sealed class CliOptions
{
    /// <summary>
    /// Gets a collection of global value converters.
    /// </summary>
    public ICollection<ValueConverter> Converters { get; set; } = new List<ValueConverter>();

    /// <summary>
    /// Gets a collection of validators.
    /// </summary>
    public ICollection<Validator> Validators { get; set; } = new List<Validator>();
}