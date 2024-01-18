namespace Vertical.Cli.Binding;

public class BindingIdComparerTests
{
    [Theory]
    [InlineData("-x", "x", true)]
    [InlineData("-x", "X", true)]
    [InlineData("--option", "option", true)]
    [InlineData("--option", "Option", true)]
    [InlineData("--long-option", "longOption", true)]
    [InlineData("--long-option", "LongOption", true)]
    [InlineData("--xX", "xx", false)]
    [InlineData("--xX", "Xx", false)]
    [InlineData("--long-option", "--long-OPTION", false)]
    public void ComparesAsExpected(string x, string y, bool expected)
    {
        Assert.Equal(expected, BindingIdComparer.Default.Equals(x, y));


    }

    [Theory]
    [InlineData("-x", "x")]
    [InlineData("-x", "X")]
    [InlineData("--option", "option")]
    [InlineData("--option", "Option")]
    [InlineData("--long-option", "longOption")]
    [InlineData("--long-option", "LongOption")]
    public void IntegratesWithHashCodes(string x, string y)
    {
        var hashSet = new HashSet<string>(BindingIdComparer.Default);
        Assert.True(hashSet.Add(x));
        Assert.False(hashSet.Add(y));   
    }
}