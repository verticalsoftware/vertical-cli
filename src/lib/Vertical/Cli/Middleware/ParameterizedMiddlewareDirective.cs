using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Middleware;

/// <summary>
/// Represents a middleware symbol that accepts a parameter value.
/// </summary>
/// <typeparam name="TValue">Value type.</typeparam>
public sealed class ParameterizedMiddlewareDirective<TValue> : MiddlewareSymbol
{
    private readonly Func<TValue>? _useDefault;
    private readonly Action<IValidationEventInfo<InvocationContext, TValue>>? _validate;
    private readonly Func<ParameterizedMiddlewareDirectiveInfo<TValue>, Task> _handler;

    /// <inheritdoc />
    internal ParameterizedMiddlewareDirective(
        string identifier,
        Func<ParameterizedMiddlewareDirectiveInfo<TValue>, Task> handler,
        Func<TValue>? useDefault,
        Action<IValidationEventInfo<InvocationContext, TValue>>? validate,
        HelpTopic? helpTopic) 
        : base(
            SymbolKind.Directive, 
            identifier,
            aliases: [],
            parameterType: typeof(TValue),
            helpTopic)
    {
        _useDefault = useDefault;
        _validate = validate;
        _handler = handler;
    }

    /// <inheritdoc />
    public override ParameterArity? ParameterArity => _useDefault is not null
        ? Configuration.ParameterArity.ZeroOrOne
        : Configuration.ParameterArity.One;

    /// <inheritdoc />
    public override async Task<int?> HandleAsync(InvocationContext context, ArgumentToken token)
    {
        switch (token.Value)
        {
            case null when _useDefault is not null:
                var infoWithValue = new ParameterizedMiddlewareDirectiveInfo<TValue>(
                    context,
                    _useDefault());
                await _handler(infoWithValue);
                return null;
            
            case null:
                context.AddError(MissingParameterError.Create(this, context.Configuration.HelpOptions.HelpProvider));
                return -1;
            
            default:
                var conversionResult = context.Configuration.TryConvertArgument<TValue>(
                    this, 
                    token.Value, 
                    context.Errors);
                
                if (!conversionResult.Success)
                    return -1;
                
                if (!Validate(context, conversionResult.Value))
                    return -1;

                await _handler(new ParameterizedMiddlewareDirectiveInfo<TValue>(
                    context,
                    conversionResult.Value));
                return null;
        }
    }

    private bool Validate(InvocationContext context, TValue parameterValue)
    {
        if (_validate is null) return true;

        var wrapper = new ValidationStateWrapper(
            context,
            this,
            _validate,
            parameterValue);

        context.AddErrors(ValidationContext.GetErrors(context, [wrapper], context));
        return context.Errors.Count == 0;
    }

    private sealed class ValidationStateWrapper(
        InvocationContext invocationContext,
        ICliSymbol symbol,
        Action<IValidationEventInfo<InvocationContext, TValue>> _validate,
        TValue value) 
        : ICliSymbol, IValidatable
    {
        /// <inheritdoc />
        public void Validate(ValidationContext context)
        {
            var info = new ValidationEventInfo<InvocationContext, TValue>(
                context,
                this,
                invocationContext,
                value);

            _validate(info);
        }

        /// <inheritdoc />
        public HelpTopic? HelpTopic => symbol.HelpTopic;

        /// <inheritdoc />
        public HelpTopicKey HelpTopicKey => symbol.HelpTopicKey;

        /// <inheritdoc />
        public string? GetRemarks() => symbol.GetRemarks();

        /// <inheritdoc />
        public IEnumerable<ExtendedRemarksSection> GetExtendedRemarksSections() => symbol.GetExtendedRemarksSections();

        /// <inheritdoc />
        public string GetListIdentifier() => symbol.GetListIdentifier();

        /// <inheritdoc />
        public string? GetParameterName() => symbol.GetParameterName();

        /// <inheritdoc />
        public ParameterArity? ParameterArity => symbol.ParameterArity;

        /// <inheritdoc />
        public string DisplayName => symbol.DisplayName;

        /// <inheritdoc />
        public SymbolKind Kind => symbol.Kind;

        /// <inheritdoc />
        public SystemKind SystemKind => symbol.SystemKind;

        /// <inheritdoc />
        public Arity Arity => symbol.Arity;
    }
}