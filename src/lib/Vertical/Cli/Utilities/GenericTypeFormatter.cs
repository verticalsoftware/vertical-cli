namespace Vertical.Cli.Utilities;

public static class GenericTypeFormatter
{
    public static string GetDisplayValue<TValue>(TValue value)
    {
        var type = value?.GetType();

        if (type == null) return "(null)";
        if (type == typeof(string) || type == typeof(char)) return $"'{value}'";
        if (type.IsValueType) return $"{value}";

        return value?.ToString() ?? type.Name;
    }

    public static string GetDisplayValues<TValue>(IEnumerable<TValue> values)
    {
        var array = values.ToArray();

        return array.Length switch
        {
            0 => "(empty)",
            1 => GetDisplayValue(array[0]),
            _ => $"[{string.Join(", ", array.Select(GetDisplayValue))}]"
        };
    }
}