using System.Text.RegularExpressions;
using Vertical.Cli.Configuration;
using Vertical.Cli.Test;

namespace Vertical.Cli.Validation;

public class ValidationBuilderExtensionsTests
{
    [Fact]
    public void MinimumLengthValidates()
    {
        AssertCase<string>(x => x.MinimumLength(5), "abcde", "abcd");
    }
    
    [Fact]
    public void MaximumLengthValidates()
    {
        AssertCase<string>(x => x.MaximumLength(5), "abcde", "abcdefg");
    }
    
    [Fact]
    public void ExactLengthValidates()
    {
        AssertCase<string>(x => x.ExactLength(5), "abcde", "abcdefg");
    }

    [Fact]
    public void LengthBetweenValidates()
    {
        AssertCase<string>(x => x.LengthBetween(3, 5), "abcd", "ab", "accdefgh");
    }

    [Fact]
    public void StartsWithValidates()
    {
        AssertCase<string>(x => x.StartsWith("abc"), "abcdef", "def");
    }
    
    [Fact]
    public void EndsWithValidates()
    {
        AssertCase<string>(x => x.EndsWith("def"), "abcdef", "abc");
    }

    [Fact]
    public void ContainsValidates()
    {
        AssertCase<string>(x => x.Contains("bcd"), "abcde", "xyz");
    }

    [Fact]
    public void MatchesStringPatternValidates()
    {
        AssertCase<string>(x => x.Matches(@"555-[\d]{4}"), "555-1212", "303-1000");
    }

    [Fact]
    public void MatchesRegexValidates()
    {
        AssertCase<string>(x => x.Matches(new Regex(@"555-[\d]{4}")), "555-1212", "303-1000");
    }

    [Fact]
    public void LessThanValidates()
    {
        AssertCase(x => x.LessThan(10), 9, 10);
    }
    
    [Fact]
    public void LessThanOrEqualsValidates()
    {
        AssertCase(x => x.LessThanOrEquals(10), 10, 11);
    }

    [Fact]
    public void GreaterThanValidates()
    {
        AssertCase(x => x.GreaterThan(10), 11, 10);
    }

    [Fact]
    public void GreaterThanOrEqualsValidates()
    {
        AssertCase(x => x.GreaterThanOrEquals(10), 10, 9);
    }

    [Fact]
    public void NotValidates()
    {
        AssertCase(x => x.Not(10), 9, 10);
    }

    [Fact]
    public void InValidates()
    {
        AssertCase(x => x.In(new[]{1,2,3}), 2, 4);
    }

    [Fact]
    public void NotInValidates()
    {
        AssertCase(x => x.NotIn(new[]{1,2,3}), 4, 2);
    }

    [Fact]
    public void FileExistsValidates()
    {
        var file = Directory.EnumerateFiles(Directory.GetCurrentDirectory()).FirstOrDefault();
        if (file == null)
            return;

        AssertCase(x => x.FileExists(), 
            new FileInfo(file),
            new FileInfo($"{Guid.NewGuid()}-{Guid.NewGuid()}.temp.test.vertical"));
    }
    
    [Fact]
    public void FilePathExistsValidates()
    {
        var file = Directory.EnumerateFiles(Directory.GetCurrentDirectory()).FirstOrDefault();
        if (file == null)
            return;
        
        AssertCase(x => x.FilePathExists(), new FileInfo(file));
    }

    [Fact]
    public void FileExistsIfNotNullValidates()
    {
        AssertCase<FileInfo?>(x => x.FileExistsIfNotNull(), null);
    }

    [Fact]
    public void DirectoryExistsValidates()
    {
        AssertCase(x => x.DirectoryExists(),
            new DirectoryInfo(Directory.GetCurrentDirectory()),
            new DirectoryInfo(Path.Combine(
                Directory.GetCurrentDirectory(),
                $"{Guid.NewGuid()}",
                $"{Guid.NewGuid()}")));
    }

    [Fact]
    public void DirectoryExistsIfNotNull()
    {
        AssertCase<DirectoryInfo?>(x => x.DirectoryExistsIfNotNull(), null);
    }
    
    private static void AssertCase<T>(
        Action<ValidationBuilder<T>> configure,
        T valid,
        params T[] invalid)
    {
        var builder = new ValidationBuilder<T>();
        configure(builder);
        
        var validator = builder.Build();
        var symbol = (SymbolDefinition<T>)Factories.CreateSymbol<T>(SymbolType.Argument, "arg");
        var list = new List<string>();
        
        validator.Validate(new ValidationContext<T>(symbol, valid, list));
        Assert.Empty(list);

        foreach (var value in invalid)
        {
            list.Clear();
            validator.Validate(new ValidationContext<T>(symbol, value, list));
            Assert.Single(list);
        }
    }
}