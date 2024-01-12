namespace Vertical.Cli.Utilities;

internal sealed class Reusable<T> where T : class
{
    private readonly Func<T> _factory;
    private readonly Action<T> _recycle;
    private object? _value;

    internal Reusable(Func<T> factory, Action<T> recycle)
    {
        _factory = factory;
        _recycle = recycle;
    }

    internal T GetInstance()
    {
        var instance = Interlocked.Exchange(ref _value, null);

        return (T?)instance ?? _factory();
    }

    internal void Return(T instance)
    {
        var recycle = Interlocked.CompareExchange(ref _value, instance, null) == null;

        if (recycle)
        {
            _recycle(instance);
        }
    }
}