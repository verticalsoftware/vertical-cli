using System.Collections.Immutable;

namespace Vertical.Cli.Binding;

public sealed partial class BindingContext
{
    /// <summary>
    /// Gets the values of the indicated binding as an array.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public TValue[] GetArray<TValue, TBinding>(string bindingName) => GetValues<TValue, TBinding>(bindingName)
        .ToArray();

    /// <summary>
    /// Gets the values of the indicated binding as a list.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public List<TValue> GetList<TValue, TBinding>(string bindingName) => GetValues<TValue, TBinding>(bindingName)
        .ToList();

    /// <summary>
    /// Gets the values of the indicated binding as a linked list.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public LinkedList<TValue> GetLinkedList<TValue, TBinding>(string bindingName) =>
        new(GetValues<TValue, TBinding>(bindingName));

    /// <summary>
    /// Gets the values of the indicated binding as a hash set.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public HashSet<TValue> GetHashSet<TValue, TBinding>(string bindingName) =>
        [..GetValues<TValue, TBinding>(bindingName)];
    
    /// <summary>
    /// Gets the values of the indicated binding as a sorted set.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public SortedSet<TValue> GetSortedSet<TValue, TBinding>(string bindingName) =>
        [..GetValues<TValue, TBinding>(bindingName)];

    /// <summary>
    /// Gets the values of the indicated binding as a stack.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public Stack<TValue> GetStack<TValue, TBinding>(string bindingName) =>
        new(GetValues<TValue, TBinding>(bindingName));
    
    /// <summary>
    /// Gets the values of the indicated binding as a queue.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public Queue<TValue> GetQueue<TValue, TBinding>(string bindingName) =>
        new(GetValues<TValue, TBinding>(bindingName));
    
    /// <summary>
    /// Gets the values of the indicated binding as an immutable array.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public ImmutableArray<TValue> GetImmutableArray<TValue, TBinding>(string bindingName) =>
        [..GetValues<TValue, TBinding>(bindingName)];
    
    /// <summary>
    /// Gets the values of the indicated binding as an immutable list.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public ImmutableList<TValue> GetImmutableList<TValue, TBinding>(string bindingName) =>
        [..GetValues<TValue, TBinding>(bindingName)];
    
    /// <summary>
    /// Gets the values of the indicated binding as an immutable hash set.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public ImmutableHashSet<TValue> GetImmutableHashSet<TValue, TBinding>(string bindingName) =>
        [..GetValues<TValue, TBinding>(bindingName)];
    
    /// <summary>
    /// Gets the values of the indicated binding as an immutable sorted set.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public ImmutableSortedSet<TValue> GetImmutableSortedSet<TValue, TBinding>(string bindingName) =>
        [..GetValues<TValue, TBinding>(bindingName)];
    
    /// <summary>
    /// Gets the values of the indicated binding as an immutable stack.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public ImmutableStack<TValue> GetImmutableStack<TValue, TBinding>(string bindingName) =>
        [..GetValues<TValue, TBinding>(bindingName)];
    
    /// <summary>
    /// Gets the values of the indicated binding as an immutable queue.
    /// </summary>
    /// <param name="bindingName">Configured binding name</param>
    /// <typeparam name="TValue">Element or collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>The value collection</returns>
    public ImmutableQueue<TValue> GetImmutableQueue<TValue, TBinding>(string bindingName) =>
        [..GetValues<TValue, TBinding>(bindingName)];
}