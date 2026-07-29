using System.Collections.Immutable;

namespace Vertical.Cli.Conversion;

public static class Converters
{
    public static ArgumentConverter<string> Default => argument => argument;

    public static ArgumentConverter<T> Parsable<T>() where T : IParsable<T> => argument => T.Parse(argument, null);

    public static ArgumentConverter<T?> NullParsable<T>() where T : struct, IParsable<T> =>
        argument => T.Parse(argument, null);

    public static ArgumentConverter<T> Enum<T>() where T : struct, Enum => argument =>
        System.Enum.Parse<T>(argument, ignoreCase: true);

    public static ArgumentConverter<T?> NullEnum<T>() where T : struct, Enum => argument =>
        System.Enum.Parse<T>(argument, ignoreCase: true);

    public static ArgumentConverter<FileInfo> FileInfo => argument => new FileInfo(argument);

    public static ArgumentConverter<DirectoryInfo> DirectoryInfo => argument => new DirectoryInfo(argument);

    public static ArgumentConverter<Uri> Uri => argument => new Uri(argument);
}