namespace Vertical.Cli.Conversion;

public class DefaultConverterTests
{
    [Fact]
    public void DefaultConverterRegistered()
    {
        Assert.NotNull(DefaultConverter<bool>.Value);
        Assert.NotNull(DefaultConverter<byte>.Value);
        Assert.NotNull(DefaultConverter<sbyte>.Value);
        Assert.NotNull(DefaultConverter<char>.Value);
        Assert.NotNull(DefaultConverter<short>.Value);
        Assert.NotNull(DefaultConverter<int>.Value);
        Assert.NotNull(DefaultConverter<long>.Value);
        Assert.NotNull(DefaultConverter<Int128>.Value);
        Assert.NotNull(DefaultConverter<ushort>.Value);
        Assert.NotNull(DefaultConverter<uint>.Value);
        Assert.NotNull(DefaultConverter<ulong>.Value);
        Assert.NotNull(DefaultConverter<UInt128>.Value);
        Assert.NotNull(DefaultConverter<Half>.Value);
        Assert.NotNull(DefaultConverter<float>.Value);
        Assert.NotNull(DefaultConverter<double>.Value);
        Assert.NotNull(DefaultConverter<decimal>.Value);
        Assert.NotNull(DefaultConverter<DateTime>.Value);
        Assert.NotNull(DefaultConverter<DateTimeOffset>.Value);
        Assert.NotNull(DefaultConverter<TimeSpan>.Value);
        Assert.NotNull(DefaultConverter<DateOnly>.Value);
        Assert.NotNull(DefaultConverter<TimeOnly>.Value);
        Assert.NotNull(DefaultConverter<Guid>.Value);
        
        Assert.NotNull(DefaultConverter<bool?>.Value);
        Assert.NotNull(DefaultConverter<byte?>.Value);
        Assert.NotNull(DefaultConverter<sbyte?>.Value);
        Assert.NotNull(DefaultConverter<char?>.Value);
        Assert.NotNull(DefaultConverter<short?>.Value);
        Assert.NotNull(DefaultConverter<int?>.Value);
        Assert.NotNull(DefaultConverter<long?>.Value);
        Assert.NotNull(DefaultConverter<Int128?>.Value);
        Assert.NotNull(DefaultConverter<ushort?>.Value);
        Assert.NotNull(DefaultConverter<uint?>.Value);
        Assert.NotNull(DefaultConverter<ulong?>.Value);
        Assert.NotNull(DefaultConverter<UInt128?>.Value);
        Assert.NotNull(DefaultConverter<Half?>.Value);
        Assert.NotNull(DefaultConverter<float?>.Value);
        Assert.NotNull(DefaultConverter<double?>.Value);
        Assert.NotNull(DefaultConverter<decimal?>.Value);
        Assert.NotNull(DefaultConverter<DateTime?>.Value);
        Assert.NotNull(DefaultConverter<DateTimeOffset?>.Value);
        Assert.NotNull(DefaultConverter<TimeSpan?>.Value);
        Assert.NotNull(DefaultConverter<DateOnly?>.Value);
        Assert.NotNull(DefaultConverter<TimeOnly?>.Value);
        Assert.NotNull(DefaultConverter<Guid?>.Value);
        
        Assert.NotNull(DefaultConverter<string>.Value);
        Assert.NotNull(DefaultConverter<FileInfo>.Value);
        Assert.NotNull(DefaultConverter<DirectoryInfo>.Value);
        Assert.NotNull(DefaultConverter<Uri>.Value);
        Assert.NotNull(DefaultConverter<ConsoleKey>.Value);
    }
}