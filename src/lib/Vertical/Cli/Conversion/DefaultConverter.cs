namespace Vertical.Cli.Conversion;

internal static class DefaultConverter
{
    internal static readonly Dictionary<Type, ValueConverter> BuiltIn = new()
    {
        [typeof(string)] = new DelegateConverter<string>(str => str),
        [typeof(bool)] = new DelegateConverter<bool>(bool.Parse),
        [typeof(byte)] = new DelegateConverter<byte>(byte.Parse),
        [typeof(sbyte)] = new DelegateConverter<sbyte>(sbyte.Parse),
        [typeof(char)] = new DelegateConverter<char>(char.Parse),
#if NET6_0_OR_GREATER
        [typeof(Half)] = new DelegateConverter<Half>(Half.Parse),
#endif
        [typeof(short)] = new DelegateConverter<short>(short.Parse),
        [typeof(int)] = new DelegateConverter<int>(int.Parse),
        [typeof(long)] = new DelegateConverter<long>(long.Parse),
#if NET7_0_OR_GREATER
        [typeof(Int128)] = new DelegateConverter<Int128>(Int128.Parse),
#endif
        [typeof(float)] = new DelegateConverter<float>(float.Parse),
        [typeof(double)] = new DelegateConverter<double>(double.Parse),
        [typeof(decimal)] = new DelegateConverter<decimal>(decimal.Parse),
        [typeof(ushort)] = new DelegateConverter<ushort>(ushort.Parse),
        [typeof(uint)] = new DelegateConverter<uint>(uint.Parse),
        [typeof(ulong)] = new DelegateConverter<ulong>(ulong.Parse),
#if NET7_0_OR_GREATER
        [typeof(UInt128)] = new DelegateConverter<UInt128>(UInt128.Parse),
#endif
        [typeof(DateTime)] = new DelegateConverter<DateTime>(DateTime.Parse),
        [typeof(DateTimeOffset)] = new DelegateConverter<DateTimeOffset>(DateTimeOffset.Parse),
        [typeof(TimeSpan)] = new DelegateConverter<TimeSpan>(TimeSpan.Parse),
#if NET6_0_OR_GREATER
        [typeof(DateOnly)] = new DelegateConverter<DateOnly>(DateOnly.Parse),
        [typeof(TimeOnly)] = new DelegateConverter<TimeOnly>(TimeOnly.Parse),
#endif
        [typeof(Guid)] = new DelegateConverter<Guid>(Guid.Parse),
        [typeof(bool?)] = new DelegateConverter<bool?>(str => bool.Parse(str)),
        [typeof(byte?)] = new DelegateConverter<byte?>(str => byte.Parse(str)),
        [typeof(sbyte?)] = new DelegateConverter<sbyte?>(str => sbyte.Parse(str)),
        [typeof(char?)] = new DelegateConverter<char?>(str => char.Parse(str)),
#if NET6_0_OR_GREATER
        [typeof(Half?)] = new DelegateConverter<Half?>(str => Half.Parse(str)),
#endif
        [typeof(short?)] = new DelegateConverter<short?>(str => short.Parse(str)),
        [typeof(int?)] = new DelegateConverter<int?>(str => int.Parse(str)),
        [typeof(long?)] = new DelegateConverter<long?>(str => long.Parse(str)),
#if NET7_0_OR_GREATER
        [typeof(Int128?)] = new DelegateConverter<Int128?>(str => Int128.Parse(str)),
#endif
        [typeof(float?)] = new DelegateConverter<float?>(str => float.Parse(str)),
        [typeof(double?)] = new DelegateConverter<double?>(str => double.Parse(str)),
        [typeof(decimal?)] = new DelegateConverter<decimal?>(str => decimal.Parse(str)),
        [typeof(ushort?)] = new DelegateConverter<ushort?>(str => ushort.Parse(str)),
        [typeof(uint?)] = new DelegateConverter<uint?>(str => uint.Parse(str)),
        [typeof(ulong?)] = new DelegateConverter<ulong?>(str => ulong.Parse(str)),
#if NET7_0_OR_GREATER
        [typeof(UInt128?)] = new DelegateConverter<UInt128?>(str => UInt128.Parse(str)),
#endif
        [typeof(DateTime?)] = new DelegateConverter<DateTime?>(str => DateTime.Parse(str)),
        [typeof(DateTimeOffset?)] = new DelegateConverter<DateTimeOffset?>(str => DateTimeOffset.Parse(str)),
        [typeof(TimeSpan?)] = new DelegateConverter<TimeSpan?>(str => TimeSpan.Parse(str)),
#if NET6_0_OR_GREATER
        [typeof(DateOnly?)] = new DelegateConverter<DateOnly?>(str => DateOnly.Parse(str)),
        [typeof(TimeOnly?)] = new DelegateConverter<TimeOnly?>(str => TimeOnly.Parse(str)),
#endif
        [typeof(Guid?)] = new DelegateConverter<Guid?>(str => Guid.Parse(str)),
        [typeof(FileInfo)] = new DelegateConverter<FileInfo>(path => new FileInfo(path)),
        [typeof(DirectoryInfo)] = new DelegateConverter<DirectoryInfo>(path => new DirectoryInfo(path)),
        [typeof(Uri)] = new DelegateConverter<Uri>(path => new Uri(path, UriKind.RelativeOrAbsolute))
    };

    internal static bool CanConvert(Type type) => type.IsEnum || BuiltIn.ContainsKey(type);
}

/// <summary>
/// Uses a <see cref="DelegateConverter{T}"/> as the implementation.
/// </summary>
internal static class DefaultConverter<T>
{
    internal static DelegateConverter<T>? Value { get; } = (DelegateConverter<T>?)TryGetInstance();

    private static ValueConverter? TryGetInstance()
    {
        if (typeof(T).IsEnum)
        {
            return new DelegateConverter<T>(str => (T)Enum.Parse(typeof(T), str, ignoreCase: true));
        }

        return DefaultConverter.BuiltIn.TryGetValue(typeof(T), out var converter)
            ? converter
            : null;
    }
}