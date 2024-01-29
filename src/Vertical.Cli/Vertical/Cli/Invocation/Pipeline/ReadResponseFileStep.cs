using Vertical.Cli.Binding;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Invocation.Pipeline;

internal sealed class ReadResponseFileStep<TResult> : CallSiteBuilderStep<TResult>
{
    /// <inheritdoc />
    internal override void Perform(RuntimeState<TResult> state, Action<RuntimeState<TResult>> next)
    {
        TryHandleResponseFileOption(state);

        next(state);
    }

    private static void TryHandleResponseFileOption(RuntimeState<TResult> state)
    {
        var option = state.BindingContext.ResponseFileOptionSymbol;

        if (option == null)
            return;

        var binding = (ArgumentBinding<FileInfo>)option.CreateBinding(state.BindingContext);
        var arguments = new List<string>(32);
        
        foreach (var path in binding.Values)
        {
            using var reader = new StreamReader(GetResponseFileStream(path));
            var tokens = ResponseFileReader.ReadTokens(reader, path.FullName);
            arguments.AddRange(tokens);
        }

        state.MergeArguments(arguments);
    }

    private static Stream GetResponseFileStream(FileInfo fileInfo)
    {
        try
        {
            return File.OpenRead(fileInfo.FullName);
        }
        catch (IOException exception)
        {
            throw InvocationExceptions.ResponseFileIOException(exception);
        }
    }
}