namespace Vertical;

public static class JsonExtensions
{
    public static VerifyJsonWriter WriteProperty(this VerifyJsonWriter writer, 
        string propertyName, 
        ReadOnlySpan<char> value)
    {
        writer.WritePropertyName(propertyName);
        writer.WriteValue($"\"{value}\"");
        return writer;
    }
    
    public static VerifyJsonWriter WriteProperty<T>(this VerifyJsonWriter writer, 
        string propertyName, 
        T value)
    {
        writer.WritePropertyName(propertyName);
        writer.WriteValue(value?.ToString() ?? "null");
        return writer;
    }
    
    public static VerifyJsonWriter WriteProperty(this VerifyJsonWriter writer, 
        string propertyName, 
        bool value)
    {
        writer.WritePropertyName(propertyName);
        writer.WriteValue(value);
        return writer;
    }
    
    public static VerifyJsonWriter WriteProperty(this VerifyJsonWriter writer, 
        string propertyName, 
        char? value)
    {
        writer.WritePropertyName(propertyName);
        writer.WriteValue(value == null ? "null" : $"'{value}'");
        return writer;
    }
    
    public static VerifyJsonWriter WriteProperty(this VerifyJsonWriter writer, 
        string propertyName, 
        int value)
    {
        writer.WritePropertyName(propertyName);
        writer.WriteValue(value);
        return writer;
    }
}