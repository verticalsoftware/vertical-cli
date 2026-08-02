using System.Collections.Immutable;

namespace Vertical.Cli.Conversion;

public static class Converters
{
    public static Converter<string, string> Default => argument => argument;

    public static Converter<string, T> Parsable<T>() where T : IParsable<T> => argument => T.Parse(argument, null);

    public static Converter<string, T?> NullParsable<T>() where T : struct, IParsable<T> =>
        argument => T.Parse(argument, null);

    public static Converter<string, T> Enum<T>() where T : struct, Enum => argument =>
        System.Enum.Parse<T>(argument, ignoreCase: true);

    public static Converter<string, T?> NullEnum<T>() where T : struct, Enum => argument =>
        System.Enum.Parse<T>(argument, ignoreCase: true);

    public static Converter<string, FileInfo> FileInfo => argument => new FileInfo(argument);

    public static Converter<string, DirectoryInfo> DirectoryInfo => argument => new DirectoryInfo(argument);

    public static Converter<string, Uri> Uri => argument => new Uri(argument);
}