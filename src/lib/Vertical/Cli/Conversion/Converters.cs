namespace Vertical.Cli.Conversion;

/// <summary>
/// Defines built-in converters.
/// </summary>
public static class Converters
{
    /// <summary>
    /// Gets the default string converter.
    /// </summary>
    public static ValueConverter<string> Default { get; } = str => str;

    /// <summary>
    /// Gets the default boolean converter.
    /// </summary>
    public static ValueConverter<bool> Boolean => Parsable<bool>();
    
    /// <summary>
    /// Gets the default boolean converter.
    /// </summary>
    public static ValueConverter<int> Integer => Parsable<int>();

    /// <summary>
    /// Gets the default <see cref="FileInfo"/> converter.
    /// </summary>
    public static ValueConverter<FileInfo> FileInfo { get; } = str => new FileInfo(str);

    /// <summary>
    /// Gets the default <see cref="DirectoryInfo"/> converter.
    /// </summary>
    public static ValueConverter<DirectoryInfo> DirectoryInfo { get; } = str => new DirectoryInfo(str);

    /// <summary>
    /// Gets the default <see cref="FileSystemInfo"/> converter.
    /// </summary>
    public static ValueConverter<FileSystemInfo> FileSystemInfo { get; } = str =>
        new DirectoryInfo(str) is { Exists: true } directoryInfo
            ? directoryInfo
            : new FileInfo(str);

    /// <summary>
    /// Gets the default <see cref="Uri"/> converter.
    /// </summary>
    public static ValueConverter<Uri> Uri { get; } = str => new Uri(str, UriKind.RelativeOrAbsolute);

    /// <summary>
    /// Gets the default <see cref="System.Version"/> converter.
    /// </summary>
    public static ValueConverter<Version> Version { get; } = System.Version.Parse;

    /// <summary>
    /// Gets the default <see cref="System.Enum"/> converter.
    /// </summary>
    public static ValueConverter<T> Enum<T>() where T : struct => EnumConverter<T>.Value;

    /// <summary>
    /// Gets the default <see cref="System.Enum"/> converter.
    /// </summary>
    public static ValueConverter<T?> NullableEnum<T>() where T : struct => NullableEnumConverter<T>.Value;

    /// <summary>
    /// Gets the default <see cref="IParsable{T}"/> converter.
    /// </summary>
    public static ValueConverter<T> Parsable<T>() where T : IParsable<T> => ParsableConverter<T>.Value;
    
    /// <summary>
    /// Gets the default <see cref="IParsable{T}"/> converter.
    /// </summary>
    public static ValueConverter<T?> NullAnnotatedParsable<T>() where T : IParsable<T> => ParsableConverter<T>.Value;

    /// <summary>
    /// Gets the default <see cref="Nullable{T}"/> converter.
    /// </summary>
    public static ValueConverter<T?> Nullable<T>() where T : struct, IParsable<T> => NullableConverter<T>.Value;

    private static class EnumConverter<T> where T : struct
    {
        public static readonly ValueConverter<T> Value = str => System.Enum.Parse<T>(str, ignoreCase: true);
    }
    
    private static class NullableEnumConverter<T> where T : struct
    {
        public static readonly ValueConverter<T?> Value = str => System.Enum.Parse<T>(str, ignoreCase: true);
    }
    
    private static class ParsableConverter<T> where T : IParsable<T>
    {
        public static readonly ValueConverter<T> Value = str => T.Parse(str, provider: null);
    }
    
    private static class NullableConverter<T> where T : struct, IParsable<T>
    {
        public static readonly ValueConverter<T?> Value = str => T.Parse(str, provider: null);
    }
}