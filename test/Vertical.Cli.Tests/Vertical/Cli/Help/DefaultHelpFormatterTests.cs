using System.Text;

namespace Vertical.Cli.Help;

[UsesVerify]
public class DefaultHelpFormatterTests
{
    [Fact]
    public Task ExpectedContentRendered()
    {
        var command = RootCommand.Create<int>(
            "grep",
            root =>
            {
                root.AddDescription("grep searches PATTERNS in each FILE. PATTERNS is one or more patterns separated " +
                                    "by newline characters, and grep prints each line that matches a pattern. Typically " +
                                    "PATTERNS should be quoted when grep is used in a shell command.");

                root.AddSwitch("-E", new[] { "--extended-regexp" },
                    description: "Interpret PATTERNS as extended regular expressions (EREs, see below).");
                root.AddSwitch("-F", new[] { "--fixed-strings" },
                    description: "Interpret PATTERNS as fixed strings, not regular expressions.");
                root.AddSwitch("-G", new[] { "--basic-regexp" },
                    description:
                    "Interpret PATTERNS as basic regular expressions (BREs, see below). This is the default.");
                root.AddOption<string>("-e", new[] { "--regexp" }, arity: Arity.OneOrMany,
                    description:
                    "Use PATTERNS as the patterns.  If this option is used multiple times or is combined " +
                    "with the -f (--file) option, search for all patterns given.  This option can be used to " +
                    "protect a pattern beginning with “-”.");
                root.AddOption<string>("-f", new[] { "--file" }, arity: Arity.ZeroOrMany,
                    description:
                    "Obtain patterns from FILE, one per line.  If this option is used multiple times or is combined " +
                    "with the -e (--regexp) option, search for all patterns given. The empty file contains zero " +
                    "patterns, and therefore matches nothing. If FILE is - , read patterns from standard input.");
                root.AddSwitch("-i", new[] { "--ignore-case" },
                    description:
                    "Ignore case distinctions in patterns and input data, so that characters that differ " +
                    "only in case match each other.");
            });
 
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        var formatter = new DefaultHelpFormatter(new DefaultHelpProvider(), writer, 80);
        
        formatter.WriteContent(command);

        var content = Encoding.UTF8.GetString(stream.ToArray());

        return Verify(content);
    }
}