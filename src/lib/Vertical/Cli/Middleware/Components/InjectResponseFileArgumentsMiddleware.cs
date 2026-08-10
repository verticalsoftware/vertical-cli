using Vertical.Cli.Diagnostics;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Middleware.Components;

internal static class InjectResponseFileArgumentsMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        for (var token = context.TokenList.First; token != null; token = token?.Next)
        {
            if (token.Kind != TokenKind.Annotation)
                continue;

            var insertPoint = token;
            token = await InjectTokensAsync(context, token);
            
            context.TokenList.Remove(insertPoint);
        }

        await next(context);
    }

    private static async Task<ArgumentToken?> InjectTokensAsync(InvocationContext context, ArgumentToken token)
    {
        try
        {
            var annotation = token.Value!;
            
            return await InjectTokensAsync(
                context,
                annotation,    
                token, 
                context.Configuration.GetAnnotationResourceStream(annotation));
        }
        catch (Exception exception)
        {
            context.AddError(new ResponseResourceError(token.Text, exception));
            return token;
        }
    }

    private static async Task<ArgumentToken?> InjectTokensAsync(
        InvocationContext context,
        string annotation,
        ArgumentToken token,
        Stream stream)
    {
        using var reader = new StreamReader(stream);

        var arguments = new List<string>();
        var lineNumber = 0;

        while (await reader.ReadLineAsync() is { } argument)
        {
            ++lineNumber;
            
            if (string.IsNullOrWhiteSpace(argument))
                continue;

            var syntaxKind = ArgumentSyntax.GetSyntaxKind(argument);
            
            switch (syntaxKind)
            {
                case SyntaxKind.Annotation:
                case SyntaxKind.OptionsTerminator:
                case SyntaxKind.Directive:
                    context.AddError(new ResponseArgumentNotSupportedError(
                        annotation, 
                        lineNumber, 
                        syntaxKind, 
                        argument));
                    break;
                
                default:
                    arguments.Add(argument);
                    break;
            }
        }

        return arguments.Count == 0
            ? token
            : context.TokenList.InsertAfter(token, arguments);
    }
}