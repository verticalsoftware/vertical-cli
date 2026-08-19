using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Invocation;

internal static class CallSite
{
    public static async Task<int> Create<TModel>(
        InvocationContext context,
        Func<HandlerServiceProvider<TModel>> handlerProvider,
        ITokenList tokenList)
        where TModel : class
    {
        var root = context.Configuration;
        
        // Build binding sources
        var modelConfiguration = root.GetModelConfiguration(typeof(TModel));
        var bindingSources = modelConfiguration.BindingSources;
        var symbols = bindingSources.OfType<CliSymbol>().ToArray();
        
        // Get the parse result of input tokens
        var parseResult = ParseResult.Create(symbols, tokenList);
        if (context.AddErrors(parseResult.GetUnresolvedTokenErrors()) > 0)
            return -1;
        
        // Get the binding results
        var bindingInfo = new PropertyBindingInfo(context, parseResult);
        
        var bindingResults = bindingSources
            .Select(source => source.CreatePropertyBinder().CreateBindingResult(bindingInfo))
            .ToDictionary(result => result.BindingName);
        
        // Validate binding results
        if (context.AddErrors(bindingResults.GetErrors()) > 0)
            return -1;

        // Create the model instance
        var bindingContext = new BindingContext<TModel>(bindingInfo, bindingResults);
        var modelBinder = modelConfiguration.GetModelBinder<TModel>();
        var modelInstance = modelBinder(bindingContext);
        
        // Run validations
        if (context.AddErrors(ValidationContext.GetErrors(context, symbols, modelInstance)) > 0)
            return -1;

        await using var handlerServiceProvider = handlerProvider();
        var handler = handlerServiceProvider.Instance;
        
        // Return an invokable task
        return await handler.HandleAsync(modelInstance, context.CancellationToken);
    }
}