namespace Vertical.Cli.Conversion;

/// <summary>
/// Out of box converters...
/// </summary>
internal static class DefaultConverter
{
    internal static readonly IReadOnlyDictionary<Type, ValueConverter> BuiltIn = new
        Dictionary<Type, ValueConverter>
        {
            [typeof(string)] = new DelegatedConverter<string>(str => str),
            [typeof(bool)] = new DelegatedConverter<bool>(bool.Parse),
            [typeof(byte)] = new DelegatedConverter<byte>(byte.Parse),
            [typeof(sbyte)] = new DelegatedConverter<sbyte>(sbyte.Parse),
            [typeof(char)] = new DelegatedConverter<char>(char.Parse),
            [typeof(short)] = new DelegatedConverter<short>(short.Parse),
            [typeof(int)] = new DelegatedConverter<int>(int.Parse),
            [typeof(long)] = new DelegatedConverter<long>(long.Parse),
            [typeof(float)] = new DelegatedConverter<float>(float.Parse),
            [typeof(double)] = new DelegatedConverter<double>(double.Parse),
            [typeof(decimal)] = new DelegatedConverter<decimal>(decimal.Parse),
            [typeof(ushort)] = new DelegatedConverter<ushort>(ushort.Parse),
            [typeof(uint)] = new DelegatedConverter<uint>(uint.Parse),
            [typeof(ulong)] = new DelegatedConverter<ulong>(ulong.Parse),
            [typeof(DateTime)] = new DelegatedConverter<DateTime>(DateTime.Parse),
            [typeof(DateTimeOffset)] = new DelegatedConverter<DateTimeOffset>(DateTimeOffset.Parse),
            [typeof(TimeSpan)] = new DelegatedConverter<TimeSpan>(TimeSpan.Parse),
            [typeof(Guid)] = new DelegatedConverter<Guid>(Guid.Parse),
            [typeof(bool?)] = new DelegatedConverter<bool?>(str => bool.Parse(str)),
            [typeof(byte?)] = new DelegatedConverter<byte?>(str => byte.Parse(str)),
            [typeof(sbyte?)] = new DelegatedConverter<sbyte?>(str => sbyte.Parse(str)),
            [typeof(char?)] = new DelegatedConverter<char?>(str => char.Parse(str)),
            [typeof(short?)] = new DelegatedConverter<short?>(str => short.Parse(str)),
            [typeof(int?)] = new DelegatedConverter<int?>(str => int.Parse(str)),
            [typeof(long?)] = new DelegatedConverter<long?>(str => long.Parse(str)),
            [typeof(float?)] = new DelegatedConverter<float?>(str => float.Parse(str)),
            [typeof(double?)] = new DelegatedConverter<double?>(str => double.Parse(str)),
            [typeof(decimal?)] = new DelegatedConverter<decimal?>(str => decimal.Parse(str)),
            [typeof(ushort?)] = new DelegatedConverter<ushort?>(str => ushort.Parse(str)),
            [typeof(uint?)] = new DelegatedConverter<uint?>(str => uint.Parse(str)),
            [typeof(ulong?)] = new DelegatedConverter<ulong?>(str => ulong.Parse(str)),
            [typeof(DateTime?)] = new DelegatedConverter<DateTime?>(str => DateTime.Parse(str)),
            [typeof(DateTimeOffset?)] = new DelegatedConverter<DateTimeOffset?>(str => DateTimeOffset.Parse(str)),
            [typeof(TimeSpan?)] = new DelegatedConverter<TimeSpan?>(str => TimeSpan.Parse(str)),
            [typeof(Guid?)] = new DelegatedConverter<Guid?>(str => Guid.Parse(str)),
            [typeof(FileInfo)] = new DelegatedConverter<FileInfo>(path => new FileInfo(path)),
            [typeof(DirectoryInfo)] = new DelegatedConverter<DirectoryInfo>(path => new DirectoryInfo(path)),
            [typeof(Uri)] = new DelegatedConverter<Uri>(path => new Uri(path, UriKind.RelativeOrAbsolute))
#if NET6_0_OR_GREATER
            ,
            [typeof(DateOnly?)] = new DelegatedConverter<DateOnly?>(str => DateOnly.Parse(str)),
            [typeof(TimeOnly?)] = new DelegatedConverter<TimeOnly?>(str => TimeOnly.Parse(str)),
            [typeof(Half)] = new DelegatedConverter<Half>(Half.Parse),
            [typeof(Half?)] = new DelegatedConverter<Half?>(str => Half.Parse(str)),
            [typeof(DateOnly)] = new DelegatedConverter<DateOnly>(DateOnly.Parse),
            [typeof(TimeOnly)] = new DelegatedConverter<TimeOnly>(TimeOnly.Parse)
#endif
#if NET7_0_OR_GREATER
            ,
            [typeof(UInt128?)] = new DelegatedConverter<UInt128?>(str => UInt128.Parse(str)),
            [typeof(Int128?)] = new DelegatedConverter<Int128?>(str => Int128.Parse(str)),
            [typeof(UInt128)] = new DelegatedConverter<UInt128>(UInt128.Parse),
            [typeof(Int128)] = new DelegatedConverter<Int128>(Int128.Parse)
#endif
        };

    internal static bool CanConvert(Type type) => type.IsEnum || BuiltIn.ContainsKey(type);
}

/// <summary>
/// Uses a <see cref="DelegatedConverter{T}"/> as the implementation.
/// </summary>
internal static class DefaultConverter<T>
{
    internal static DelegatedConverter<T>? Value { get; } = (DelegatedConverter<T>?)TryGetInstance();

    private static ValueConverter? TryGetInstance()
    {
        if (typeof(T).IsEnum)
        {
            return new DelegatedConverter<T>(str => (T)Enum.Parse(typeof(T), str, ignoreCase: true));
        }

        return DefaultConverter.BuiltIn.TryGetValue(typeof(T), out var converter)
            ? converter
            : null;
    }
}