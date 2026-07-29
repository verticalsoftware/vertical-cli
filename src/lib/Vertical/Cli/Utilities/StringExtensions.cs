using System.Text;

namespace Vertical.Cli.Utilities;

internal static class StringExtensions
{
    extension(string str)
    {
        public string ToKebabCase(string? prefix = null, bool toUpperCase = false, char separator = '-')
        {
            var sb = new StringBuilder(prefix ?? string.Empty);
            var len = str.Length;
            var c = 0;
            
            Func<char, char> converter = toUpperCase
                ? char.ToUpper
                : char.ToLower;

            while (c < len)
            {
                var ch = str[c];

                if (IsGroupable(ch))
                {
                    var count = 0;
                    if (c > 0) sb.Append(separator);

                    do
                    {
                        sb.Append(converter(ch));
                        ++count;
                    } while (++c < len && IsGroupable(ch = str[c]));

                    if (c >= len)
                        break;

                    if (count > 1) sb.Append(separator);
                    continue;
                }

                sb.Append(converter(ch));
                ++c;
            }

            return sb.ToString();

            static bool IsGroupable(char ch)
            {
                return char.IsUpper(ch) || char.IsDigit(ch);
            }
        }
    }
}