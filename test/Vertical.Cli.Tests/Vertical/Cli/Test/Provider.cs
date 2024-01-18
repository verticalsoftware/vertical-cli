namespace Vertical.Cli.Test;

public class Provider<T>
{
    private readonly T _value;

    public Provider(T value)
    {
        _value = value;
    }

    public T GetValue()
    {
        Called = true;
        return _value;
    }
    
    public bool Called { get; private set; }
    
    public static implicit operator Func<T>(Provider<T> provider)
    {
        return provider.GetValue;
    }
}