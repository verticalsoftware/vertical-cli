using Shouldly;

namespace Vertical.Cli.Internal;

public class StringExtensionsTests
{
    public sealed class KebabTestData : TheoryData<string, string>
    {
        public KebabTestData()
        {
            Add("Binding", "binding");
            Add("BindingName", "binding-name");
            Add("ComplexBindingName", "complex-binding-name");
        }
    }

    [Theory, ClassData(typeof(KebabTestData))]
    public void ToKebabCase_Returns_Expected(string input, string expected)
    {
        input.ToKebabCase(ReadOnlySpan<char>.Empty).ShouldBe(expected);
    }

    public sealed class SnakeCaseData : TheoryData<string, string>
    {
        public SnakeCaseData()
        {
            Add("Binding", "BINDING");
            Add("BindingName", "BINDING_NAME");
            Add("BindingComplexName", "BINDING_COMPLEX_NAME");
            Add("", "");
        }

        [Theory, ClassData(typeof(SnakeCaseData))]
        public void ToUpperSnakeCase_Returns_Expected(string input, string expected)
        {
            input.ToUpperSnakeCase().ShouldBe(expected);
        }
    }
}