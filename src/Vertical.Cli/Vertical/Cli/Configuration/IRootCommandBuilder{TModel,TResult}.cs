﻿namespace Vertical.Cli.Configuration;

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
    /// <param name="returnValue">The value to return from the command handler.</param>
    /// <returns>A reference to this instance.</returns>
    ICommandBuilder<TModel, TResult> AddHelpOption(
        string id = "--help",
        string[]? aliases = null,
        string description = "Display help content.",
        TResult returnValue = default!);
}