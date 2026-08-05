namespace Vertical.Cli.Conversion;

/// <summary>
/// Defines argument converters.
/// </summary>
public static class Converters
{
    /// <summary>
    /// Defines the default string converter.
    /// </summary>
    public static Converter<string, string> Default => argument => argument;

    /// <summary>
    /// Creates a converter for types that implement <see cref="IParsable{TSelf}"/>
    /// </summary>
    /// <typeparam name="T">Underlying type</typeparam>
    public static Converter<string, T> Parsable<T>() where T : IParsable<T> => argument => T.Parse(argument, null);

    /// <summary>
    /// Creates a converter for nullable value types that implement <see cref="IParsable{TSelf}"/>
    /// </summary>
    /// <typeparam name="T">Underlying type</typeparam>
    public static Converter<string, T?> NullParsable<T>() where T : struct, IParsable<T> =>
        argument => T.Parse(argument, null);

    /// <summary>
    /// Creates a converter for an  <see cref="System.Enum"/> type.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public static Converter<string, T> Enum<T>() where T : struct, Enum => argument =>
        System.Enum.Parse<T>(argument, ignoreCase: true);

    /// <summary>
    /// Creates a converter for a nullable <see cref="System.Enum"/> type.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public static Converter<string, T?> NullEnum<T>() where T : struct, Enum => argument =>
        System.Enum.Parse<T>(argument, ignoreCase: true);

    /// <summary>
    /// Creates a converter for the <see cref="FileInfo"/> type.
    /// </summary>
    public static Converter<string, FileInfo> FileInfo => argument => new FileInfo(argument);

    /// <summary>
    /// Creates a converter for the <see cref="DirectoryInfo"/> type.
    /// </summary>
    public static Converter<string, DirectoryInfo> DirectoryInfo => argument => new DirectoryInfo(argument);

    /// <summary>
    /// Creates a converter for the <see cref="Uri"/> type.
    /// </summary>
    public static Converter<string, Uri> Uri => argument => new Uri(argument);
}