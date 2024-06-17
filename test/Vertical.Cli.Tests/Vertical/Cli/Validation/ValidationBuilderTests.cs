using System.Diagnostics.CodeAnalysis;
using Shouldly;

namespace Vertical.Cli.Validation;

public interface IValidatorCase
{
    void Assert();
}

public class ValidatorCase<T>(
    Action<ValidationBuilder<Model<T>, T>> configure,
    T value,
    bool isValid)
    : IValidatorCase
{
    public void Assert()
    {
        var builder = new ValidationBuilder<Model<T>, T>();
        configure(builder);
        var function = builder.Build();
        var result = function(new Model<T>(value), value);
        result.IsValid.ShouldBe(isValid);
    }
} 

[SuppressMessage("Usage", "xUnit1045:Avoid using TheoryData type arguments that might not be serializable")]
public class ValidationBuilderTests
{
    private static readonly FileInfo KnownFile = new(Directory.EnumerateFiles(Directory.GetCurrentDirectory()).First());

    private static readonly DirectoryInfo KnownDirectory = new(Directory.GetCurrentDirectory());

    private static readonly FileInfo UnknownFile =
        new(Path.Combine(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}.txt"));

    private static readonly DirectoryInfo UnknownDirectory =
        new(Path.Combine(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}"));

    [Theory, MemberData(nameof(Theories))]
    public static void Validation_Returns_Expected(IValidatorCase obj) => obj.Assert();

    public static TheoryData<IValidatorCase> Theories => new TheoryData<IValidatorCase>(
    [
        // Positive cases
        new ValidatorCase<int>(c => c.LessThan(1), 0, true),
        new ValidatorCase<int>(c => c.LessThanOrEqual(1), 1, true),
        new ValidatorCase<int>(c => c.GreaterThan(1), 2, true),
        new ValidatorCase<int>(c => c.GreaterThanOrEqual(1), 1, true),
        new ValidatorCase<int?>(c => c.LessThan(1), 0, true),
        new ValidatorCase<int?>(c => c.LessThanOrEqual(1), 1, true),
        new ValidatorCase<int?>(c => c.GreaterThan(1), 2, true),
        new ValidatorCase<int?>(c => c.GreaterThanOrEqual(1), 1, true),
        new ValidatorCase<int?>(c => c.LessThan(1), null, true),
        new ValidatorCase<int?>(c => c.LessThanOrEqual(1), null, true),
        new ValidatorCase<int?>(c => c.GreaterThan(1), null, true),
        new ValidatorCase<int?>(c => c.GreaterThanOrEqual(1), null, true),
        new ValidatorCase<string>(c => c.HasMinLength(1), "a", true),
        new ValidatorCase<string>(c => c.HasMaxLength(3), "abc", true),
        new ValidatorCase<string?>(c => c.HasMinLengthOrIsNull(1), "a", true),
        new ValidatorCase<string?>(c => c.HasMaxLengthOrIsNull(3), "abc", true),
        new ValidatorCase<string?>(c => c.HasMinLengthOrIsNull(1), null, true),
        new ValidatorCase<string?>(c => c.HasMaxLengthOrIsNull(3), null, true),
        new ValidatorCase<string>(c => c.Matches("[abc]{3}"), "abc", true),
        new ValidatorCase<string>(c => c.DoesNotMatch("[abc]{3}"), "def", true),
        new ValidatorCase<string?>(c => c.MatchesOrIsNull("[abc]{3}"), "abc", true),
        new ValidatorCase<string?>(c => c.DoesNotMatchOrIsNull("[abc]{3}"), "def", true),
        new ValidatorCase<string?>(c => c.MatchesOrIsNull("[abc]{3}"), null, true),
        new ValidatorCase<string?>(c => c.DoesNotMatchOrIsNull("[abc]{3}"), null, true),
        new ValidatorCase<string>(c => c.In(["red", "green", "blue"]), "red", true),
        new ValidatorCase<string>(c => c.NotIn(["red", "green", "blue"]), "pink", true),
        new ValidatorCase<string?>(c => c.InOrIsNull(["red", "green", "blue"]), "red", true),
        new ValidatorCase<string?>(c => c.NotInOrIsNull(["red", "green", "blue"]), "pink", true),
        new ValidatorCase<string?>(c => c.InOrIsNull(["red", "green", "blue"]), null, true),
        new ValidatorCase<string?>(c => c.NotInOrIsNull(["red", "green", "blue"]), null, true),
        new ValidatorCase<FileInfo>(c => c.Exists(), KnownFile, true),
        new ValidatorCase<FileInfo?>(c => c.ExistsOrIsNull(), KnownFile, true),
        new ValidatorCase<FileInfo?>(c => c.ExistsOrIsNull(), null, true),
        new ValidatorCase<DirectoryInfo>(c => c.Exists(), KnownDirectory, true),
        new ValidatorCase<DirectoryInfo?>(c => c.ExistsOrIsNull(), KnownDirectory, true),
        new ValidatorCase<DirectoryInfo?>(c => c.ExistsOrIsNull(), null, true),
        new ValidatorCase<FileInfo>(c => c.DoesNotExist(), UnknownFile, true),
        new ValidatorCase<FileInfo?>(c => c.DoesNotExistOrIsNull(), UnknownFile, true),
        new ValidatorCase<FileInfo?>(c => c.DoesNotExistOrIsNull(), null, true),
        new ValidatorCase<DirectoryInfo>(c => c.DoesNotExist(), UnknownDirectory, true),
        new ValidatorCase<DirectoryInfo?>(c => c.DoesNotExistOrIsNull(), UnknownDirectory, true),
        new ValidatorCase<DirectoryInfo?>(c => c.DoesNotExistOrIsNull(), null, true),
        
        // Negative cases
        new ValidatorCase<int>(c => c.LessThan(1), 1, false),
        new ValidatorCase<int>(c => c.LessThanOrEqual(1), 2, false),
        new ValidatorCase<int>(c => c.GreaterThan(1), 1, false),
        new ValidatorCase<int>(c => c.GreaterThanOrEqual(1), 0, false),
        new ValidatorCase<int?>(c => c.LessThan(1), 1, false),
        new ValidatorCase<int?>(c => c.LessThanOrEqual(1), 2, false),
        new ValidatorCase<int?>(c => c.GreaterThan(1), 1, false),
        new ValidatorCase<int?>(c => c.GreaterThanOrEqual(1), 0, false),
        new ValidatorCase<string>(c => c.HasMinLength(3), "a", false),
        new ValidatorCase<string>(c => c.HasMaxLength(3), "abcde", false),
        new ValidatorCase<string>(c => c.Matches("[abc]{3}"), "def", false),
        new ValidatorCase<string>(c => c.DoesNotMatch("[abc]{3}"), "abc", false),
        new ValidatorCase<string>(c => c.In(["red", "green", "blue"]), "pink", false),
        new ValidatorCase<string>(c => c.NotIn(["red", "green", "blue"]), "red", false),
        new ValidatorCase<FileInfo>(c => c.Exists(), UnknownFile, false),
        new ValidatorCase<DirectoryInfo>(c => c.Exists(), UnknownDirectory, false),
        new ValidatorCase<FileInfo>(c => c.DoesNotExist(), KnownFile, false),
        new ValidatorCase<DirectoryInfo>(c => c.DoesNotExist(), KnownDirectory, false),
    ]);

    [Fact]
    public void Each_Iterates_And_Validates_Elements()
    {
        var unit = new ValidationBuilder<Model<string[]>, string[]>();
        unit.Each<string>(b => b.HasMinLength(1));

        var function = unit.Build();
        var model = new Model<string[]>(["test"]);
        
        function(model, model.Value).IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Each_Throws_For_Non_Enumerable()
    {
        var unit = new ValidationBuilder<Model<string>, string>();
        Should.Throw<ArgumentException>(() => unit.Each<string>(v => { }));
    }
}