using System.Text;

namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Base class for configuration assertions.
/// </summary>
public abstract class ConfigurationAssertion
{
    /// <summary>
    /// Gets a key that can be used to group the assertion.
    /// </summary>
    public abstract string GroupingKey { get; }

    /// <summary>
    /// Gets the issue description.
    /// </summary>
    /// <returns></returns>
    public abstract string GetIssueDescription();

    /// <summary>
    /// Gets zero or more detail items about the assertion.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<string> GetIssueDetail() => [];
    
    /// <summary>
    /// Returns any assertions detected in the configuration of the given application as a string.
    /// </summary>
    /// <param name="assertions">An enumeration of collected assertions.</param>
    /// <returns>A string containing the assertion stream or <c>null</c> if no errors were found..</returns>
    public static string? GetAssertionsAsText(IEnumerable<ConfigurationAssertion> assertions)
    {
        var sb = new StringBuilder(5000);
        var newLine = false;

        foreach (var grouping in assertions.GroupBy(assertion => assertion.GroupingKey).OrderBy(group => group.Key))
        {
            if (newLine) sb.AppendLine();
            
            sb.Append(grouping.Key);
            sb.AppendLine(":");
            
            foreach (var assertion in grouping)
            {
                sb.Append(' ', 2);
                sb.AppendLine(assertion.GetIssueDescription());
                foreach (var detail in assertion.GetIssueDetail())
                {
                    sb.Append(' ', 4);
                    sb.AppendLine(detail);
                }
            }

            newLine = true;
        }

        return sb.Length == 0 ? null : sb.ToString();
    }
}