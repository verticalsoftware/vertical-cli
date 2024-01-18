//HintName: RootCommandExtensions.g.cs
/*
Copyright (C) 2023-2024 Vertical Software

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the “Software”), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;

namespace Vertical.Cli;

/// <summary>
/// Defines methods to invoke and validate a root command.
/// </summary>
public static class RootCommandExtensions
{
        /// <summary>
        /// Invokes the selected command handler function using the path and values
        /// provided by the programs string arguments.
        /// </summary>
        /// <param name="rootCommand">The root command instance.</param>
        /// <param name="args">The collection of program arguments.</param>
        /// <returns>The result provided by the command handler implementation.</returns>
    public static async global::System.Threading.Tasks.Task<int> InvokeAsync(
        this global::Vertical.Cli.IRootCommand<
            global::TestSetup.FileCopyParameters,
            global::System.Threading.Tasks.Task<int>> rootCommand,
        global::System.Collections.Generic.IEnumerable<string> args,
        global::System.Threading.CancellationToken cancellationToken = default)
    {
        if (rootCommand == null)
        {
            throw new global::System.ArgumentNullException(nameof(rootCommand));
        }

        if (args == null)
        {
            throw new global::System.ArgumentNullException(nameof(args));
        }

#if ((DEBUG || ENABLE_CLI_VALIDATION) && !DISABLE_CLI_VALIDATION)
        global::Vertical.Cli.Configuration.ConfigurationValidator.ThrowIfInvalid(rootCommand);
#endif

        var context = global::Vertical.Cli.Invocation.CallSiteContext.Create(
            rootCommand,
            args,
            global::Vertical.Cli.Binding.DefaultOf<int>.TaskValue);

        var callsite = GetCallSite(context);
        return await callsite(cancellationToken);
    }

    private static global::System.Func<
        global::System.Threading.CancellationToken,
        global::System.Threading.Tasks.Task<int>>
        GetCallSite(global::Vertical.Cli.Invocation.ICallSiteContext<global::System.Threading.Tasks.Task<int>> context)
    {
        var callSite = context.CallSite;
        var modelType = callSite.ModelType;

        if (modelType == typeof(global::Vertical.Cli.None))
        {
            return context.BindModelToCallSite<global::Vertical.Cli.None>();
        }

        if (modelType == typeof(global::TestSetup.FileCopyParameters))
        {
            return context.BindModelToCallSite(bindingContext =>
                new global::TestSetup.FileCopyParameters(
                    Source: bindingContext.GetValue<FileInfo>("Source"),
                    Dest: bindingContext.GetValue<FileInfo>("Dest"),
                    Compression: bindingContext.GetValue<global::TestSetup.Compression>("Compression"),
                    Overwrite: bindingContext.GetValue<bool>("Overwrite")));
        }

        throw new global::System.InvalidOperationException();
    }
}
