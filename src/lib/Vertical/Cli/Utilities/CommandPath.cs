﻿using System.Collections.Immutable;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Utilities;

/// <summary>
/// Defines the data and commands of an execution path.
/// </summary>
/// <typeparam name="TResult">Command result type.</typeparam>
public class CommandPath<TResult>
{
    private record LazyData(
        IReadOnlyCollection<SymbolDefinition> Symbols,
        IReadOnlyCollection<ValueConverter> Converters,
        IReadOnlyCollection<Validator> Validators);

    private readonly Lazy<LazyData> _lazyData;
    
    internal CommandPath(ICommandDefinition<TResult> command)
        : this(null, command)
    {
    }

    internal CommandPath(ImmutableArray<ICommandDefinition<TResult>> commands)
    {
        Commands = commands;
        Subject = Commands.Last();
        _lazyData = new Lazy<LazyData>(InitializeLazyData);
    }

    private CommandPath(CommandPath<TResult>? basePath, ICommandDefinition<TResult> command)
        : this(basePath?.Commands.Add(command) ?? ImmutableArray.Create(command))
    {
    }

    /// <summary>
    /// Gets the sub-paths for the subject command.
    /// </summary>
    public IEnumerable<CommandPath<TResult>> SubPaths => Subject
        .SubCommands
        .Select(command => new CommandPath<TResult>(this, command));

    /// <summary>
    /// Gets an array of command definitions that represents the current command path.
    /// </summary>
    public ImmutableArray<ICommandDefinition<TResult>> Commands { get; }

    /// <summary>
    /// Gets the subject command.
    /// </summary>
    public ICommandDefinition<TResult> Subject { get; }

    /// <summary>
    /// Gets an instance of this type that contains only the subject.
    /// </summary>
    public CommandPath<TResult> SubjectPath => new(null, Subject);

    /// <summary>
    /// Gets the symbols in the command path.
    /// </summary>
    public IReadOnlyCollection<SymbolDefinition> Symbols => _lazyData.Value.Symbols;

    /// <summary>
    /// Gets the converters within the path.
    /// </summary>
    public IReadOnlyCollection<ValueConverter> Converters => _lazyData.Value.Converters;

    /// <summary>
    /// Gets the validators within the path.
    /// </summary>
    public IReadOnlyCollection<Validator> Validators => _lazyData.Value.Validators;

    /// <inheritdoc />
    public override string ToString() => Subject.GetPathString();

    private LazyData InitializeLazyData()
    {
        return new LazyData(
            AggregateSymbols(),
            AggregateServices(command => command.Converters),
            AggregateServices(command => command.Validators)
        );
    }

    private IReadOnlyCollection<SymbolDefinition> AggregateSymbols()
    {
        return Commands
            .SkipLast(1)
            .SelectMany(command => command.Symbols.Where(symbol => symbol.Scope is
                SymbolScope.SelfAndDescendents or SymbolScope.Descendents))
            .Concat(Subject.Symbols.Where(symbol => symbol.Scope is SymbolScope.Self or SymbolScope.SelfAndDescendents))
            .ToArray();
    }

    private IReadOnlyCollection<T> AggregateServices<T>(
        Func<ICommandDefinition<TResult>, IEnumerable<T>> serviceSelector)
    {
        return Commands.SelectMany(serviceSelector).ToArray();
    }
}