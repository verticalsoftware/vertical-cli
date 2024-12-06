using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Routing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents information used for model binding and invocation of
/// command handlers.
/// </summary>
public sealed partial class BindingContext
{
    private BindingContext(CliApplication application,
        IReadOnlyList<ArgumentSyntax> argumentList,
        RouteTarget routeTarget,
        BindingValuesLookup bindingValues, 
        IReadOnlyDictionary<string, IBindingSource> providedBindings,
        Delegate? callSite)
    {
        Application = application;
        ArgumentList = argumentList;
        RouteDefinition = routeTarget.Route;
        RouteArguments = routeTarget.Arguments;
        BindingValues = bindingValues;
        ProvidedBindings = providedBindings;
        CallSite = callSite;
        Parameters = bindingValues.Parameters;
        ValueConverters = application.Converters.ToDictionary(c => c.ValueType);
    }

    private Delegate? CallSite { get; }

    /// <summary>
    /// Gets the <see cref="CliApplication"/> instance.
    /// </summary>
    public CliApplication Application { get; }

    /// <summary>
    /// Gets the parsed arguments.
    /// </summary>
    public IReadOnlyList<ArgumentSyntax> ArgumentList { get; }

    /// <summary>
    /// Gets the route arguments.
    /// </summary>
    public IReadOnlyList<ArgumentSyntax> RouteArguments { get; set; }

    /// <summary>
    /// Gets the current route definition.
    /// </summary>
    public RouteDefinition RouteDefinition { get; }

    /// <summary>
    /// Gets the collection of mapped parameters (options, switches, and arguments).
    /// </summary>
    public ParameterCollection Parameters { get; }

    /// <summary>
    /// Gets a lookup of binding values paired with identifiers of the parameters they are
    /// associated with.
    /// </summary>
    public BindingValuesLookup BindingValues { get; }

    /// <summary>
    /// Gets bindings provided by the application.
    /// </summary>
    public IReadOnlyDictionary<string, IBindingSource> ProvidedBindings { get; }

    /// <summary>
    /// Gets the value converters registered by the application or automatically
    /// by the source generator.
    /// </summary>
    public IDictionary<Type, IValueConverter> ValueConverters { get; }

    /// <summary>
    /// Adds a <see cref="IValueConverter"/> instance if the target value type is not already
    /// provided for.
    /// </summary>
    /// <param name="converter">The converter instance to add.</param>
    public void TryAddConverter(IValueConverter converter) => ValueConverters.TryAdd(
        converter.ValueType, converter);

    /// <summary>
    /// Attempts to invoke a call site that has internal model binding.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation.</param>
    /// <returns>The integer result or <c>null</c> if the site was not invoked.</returns>
    public async Task<int?> TryInvokeInternalCallSite(CancellationToken cancellationToken)
    {
        if (TryGetCallSite<BindingContext>(out var bindingCallSite))
            return await bindingCallSite(this, cancellationToken);

        if (TryGetCallSite<EmptyModel>(out var emptyCallSite))
            return await emptyCallSite(EmptyModel.Instance, cancellationToken);

        return null;
    }

    /// <summary>
    /// Tries to get a call site for a particular model type.
    /// </summary>
    /// <param name="callSite">When the method returns and a callsite is available for the given
    /// model type, a reference to the call site.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns><c>true</c> if <paramref name="callSite"/> was assigned.</returns>
    public bool TryGetCallSite<TModel>([NotNullWhen(true)] out AsyncCallSite<TModel>? callSite) where TModel : class
    {
        if (CallSite is AsyncCallSite<TModel> target)
        {
            callSite = WrapCallSite(target);
            return true;
        }

        callSite = default;
        return false;
    }

    /// <summary>
    /// Gets a single binding value.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>Value of <typeparamref name="T"/>, or <c>default</c>.</returns>
    public T GetValue<T>(string bindingName) => SelectSingleValue<T>(bindingName, BindingValues[bindingName]);

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>A enumerable of zero, one, or more values of <typeparamref name="T"/></returns>
    public IEnumerable<T> GetValues<T>(string bindingName)
    {
        return SelectCollectionValues<T>(bindingName, BindingValues[bindingName]);
    }

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>A <see cref="Array"/> of zero, one, or more values of <typeparamref name="T"/></returns>
    public T[] GetArray<T>(string bindingName) => GetValues<T>(bindingName).ToArray();

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>A <see cref="HashSet{T}"/> of zero, one, or more values of <typeparamref name="T"/></returns>
    public HashSet<T> GetHashSet<T>(string bindingName) => [..GetValues<T>(bindingName)];

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>A <see cref="SortedSet{T}"/> of zero, one, or more values of <typeparamref name="T"/></returns>
    public HashSet<T> GetSortedSet<T>(string bindingName) => [..GetValues<T>(bindingName)];

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>A <see cref="List{T}"/> of zero, one, or more values of <typeparamref name="T"/></returns>
    public List<T> GetList<T>(string bindingName) => [..GetValues<T>(bindingName)];

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>A <see cref="LinkedList{T}"/> of zero, one, or more values of <typeparamref name="T"/></returns>
    public List<T> GetLinkedList<T>(string bindingName) => [..GetValues<T>(bindingName)];

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>A <see cref="Stack{T}"/> of zero, one, or more values of <typeparamref name="T"/></returns>
    public Stack<T> GetStack<T>(string bindingName) => new(GetValues<T>(bindingName));

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>A <see cref="Queue{T}"/> of zero, one, or more values of <typeparamref name="T"/></returns>
    public Queue<T> GetQueue<T>(string bindingName) => new(GetValues<T>(bindingName));

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>
    /// A <see cref="System.Collections.Immutable.ImmutableArray{T}"/> of zero, one, or more values of
    /// <typeparamref name="T"/>
    /// </returns>
    public ImmutableArray<T> GetImmutableArray<T>(string bindingName) => [..GetValues<T>(bindingName)];

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>
    /// A <see cref="System.Collections.Immutable.ImmutableHashSet{T}"/> of zero, one, or more values of
    /// <typeparamref name="T"/>
    /// </returns>
    public ImmutableHashSet<T> GetImmutableHashSet<T>(string bindingName) => [..GetValues<T>(bindingName)];

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>
    /// A <see cref="System.Collections.Immutable.ImmutableSortedSet{T}"/> of zero, one, or more values of
    /// <typeparamref name="T"/>
    /// </returns>
    public ImmutableHashSet<T> GetImmutableSortedSet<T>(string bindingName) => [..GetValues<T>(bindingName)];

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>
    /// A <see cref="System.Collections.Immutable.ImmutableStack{T}"/> of zero, one, or more values of
    /// <typeparamref name="T"/>
    /// </returns>
    public ImmutableStack<T> GetImmutableStack<T>(string bindingName) => [..GetValues<T>(bindingName)];

    /// <summary>
    /// Gets a collection of binding values.
    /// </summary>
    /// <param name="bindingName">The name of the binding.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>
    /// A <see cref="System.Collections.Immutable.ImmutableQueue{T}"/> of zero, one, or more values of
    /// <typeparamref name="T"/>
    /// </returns>
    public ImmutableQueue<T> GetImmutableQueue<T>(string bindingName) => [..GetValues<T>(bindingName)];

    internal static BindingContext Create(
        CliApplication application,
        IReadOnlyList<ArgumentSyntax> arguments,
        RouteTarget routeTarget,
        Delegate? callSite)
    {
        var valueLookup = BindingValuesLookup.Create(application, routeTarget);
        var bindings = GetProvidedValueBindings(application, routeTarget);

        return new BindingContext(
            application,
            arguments,
            routeTarget,
            valueLookup,
            bindings,
            callSite);
    }

    private static Dictionary<string, IBindingSource> GetProvidedValueBindings(
        CliApplication application, 
        RouteTarget routeTarget)
    {
        var dictionary = new Dictionary<string, IBindingSource>();

        foreach (var type in routeTarget.Route.ModelType.GetTypeFamily())
        {
            if (!application.ModelConfigurations.TryGetValue(type, out var configuration))
                continue;

            foreach (var (key, value) in configuration.BindingSources)
            {
                dictionary.Add(key, value);
            }
        }

        return dictionary;
    }

    private AsyncCallSite<TModel> WrapCallSite<TModel>(AsyncCallSite<TModel> target) where TModel : class
    {
        return async (model, token) =>
        {
            ValidateCallSite(model);
            return await target(model, token);
        };
    }

    private void ValidateCallSite<TModel>(TModel model) where TModel : class
    {
        foreach (var type in typeof(TModel).GetTypeFamily())
        {
            if (!Application.ModelConfigurations.TryGetValue(type, out var configuration))
                continue;
            
            configuration.Validate(this, model);
        }
    }
}