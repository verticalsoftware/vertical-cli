using Vertical.Cli.Help;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a builder used to configure the root command.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface IRootCommandBuilder<out TModel, TResult> : ICommandBuilder<TModel, TResult>
    where TModel : class
{
    /// <summary>
    /// Adds a help option.
    /// </summary>
    /// <param name="id">The primary identity for the option.</param>
    /// <param name="aliases">An optional array of alias identities.</param>
    /// <param name="description">The description to display for this option.</param>
    /// <param name="formatterProvider">A function that provides a <see cref="IHelpFormatter"/> instance
    /// at the time it is needed. If <c>null</c>, a default formatter is used.</param>
    /// <returns>A reference to this instance.</returns>
    ICommandBuilder<TModel, TResult> SetHelpOption(
        string id = "--help",
        string[]? aliases = null,
        string description = "Display help content.",
        Func<IHelpFormatter>? formatterProvider = null);

    /// <summary>
    /// Adds a response file option.
    /// </summary>
    /// <param name="id">The primary identity for the option.</param>
    /// <param name="aliases">An optional array of alias identities.</param>
    /// <param name="arity">An arity that describes the use of the option.</param>
    /// <param name="description">A description of the option.</param>
    /// <param name="defaultProvider">A function that provides a default value.</param>
    /// <param name="validator">An object that performs cursory validation of the value.</param>
    ICommandBuilder<TModel, TResult> SetResponseFileOption(
        string id = "--silent",
        string[]? aliases = null,
        Arity? arity = null,
        string description = "Response file to read for unattended input.",
        Func<FileInfo>? defaultProvider = null,
        Validator<FileInfo>? validator = null);
}