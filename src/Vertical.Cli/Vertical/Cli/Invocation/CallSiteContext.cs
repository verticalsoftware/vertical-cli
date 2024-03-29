﻿using CommunityToolkit.Diagnostics;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation.Pipeline;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Creates a <see cref="ICallSiteContext{TResult}"/> from a root command definition and program arguments.
/// </summary>
public static class CallSiteContext
{
    /// <summary>
    /// Creates a call site.
    /// </summary>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="args">Application arguments.</param>
    /// <param name="defaultValue">The default value to return for help or error sites.</param>
    /// <typeparam name="TModel">Root command model type.</typeparam>
    /// <typeparam name="TResult">Application result type.</typeparam>
    /// <returns><see cref="ICallSiteContext{TResult}"/></returns>
    public static ICallSiteContext<TResult> Create<TModel, TResult>(
        IRootCommand<TModel, TResult> rootCommand,
        IEnumerable<string> args,
        TResult defaultValue)
        where TModel : class
    {
        Guard.IsNotNull(rootCommand);
        Guard.IsNotNull(args);

        var selectedPath = CommandSelector.GetPath(rootCommand, args);
        var bindingContext = new RuntimeBindingContext(
            rootCommand.Options, 
            selectedPath.Subject, 
            args, 
            selectedPath.Arguments);
        
        return CallSiteBuilder.Build(bindingContext, rootCommand.Options, defaultValue);
    }
}