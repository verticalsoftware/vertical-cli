using System.Text.RegularExpressions;

namespace Vertical.Cli.Validation;

public static class StringValidationExtensions
{
    extension<TModel>(ValidationEventInfo<TModel, string> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a minimum string length check.
        /// </summary>
        /// <param name="length">The minimum string length to require.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Length is less than 1.</exception>
        public ValidationEventInfo<TModel, string> MustBeOfLength(int length, Func<string>? formatMessage = null)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(length, 1);

            return eventInfo.Value.Length >= length
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be {length} character(s).");
        }
        
        /// <summary>
        /// Adds a maximum string length check.
        /// </summary>
        /// <param name="length">The maximum string length to allow.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Length is less than 1.</exception>
        public ValidationEventInfo<TModel, string> MustNotExceedLength(int length, Func<string>? formatMessage = null)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(length, 1);

            return eventInfo.Value.Length >= length
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be {length} character(s).");
        }
        
        /// <summary>
        /// Adds a regular expression pattern check.
        /// </summary>
        /// <param name="pattern">The regular expression to match.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public ValidationEventInfo<TModel, string> MustMatchPattern(string pattern, Func<string>? formatMessage = null)
        {
            ArgumentNullException.ThrowIfNull(pattern);

            return Regex.IsMatch(eventInfo.Value, pattern)
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must match pattern {pattern}.");
        }

        /// <summary>
        /// Adds a regular expression pattern check.
        /// </summary>
        /// <param name="regex">The regular expression to match.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public ValidationEventInfo<TModel, string> MustMatchPattern(Regex regex, Func<string>? formatMessage = null)
        {
            ArgumentNullException.ThrowIfNull(regex);

            return regex.IsMatch(eventInfo.Value)
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must match pattern {regex}.");
        }
    }
    
    extension<TModel>(ValidationEventInfo<TModel, string?> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a minimum string length check.
        /// </summary>
        /// <param name="length">The minimum string length to require.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Length is less than 1.</exception>
        public ValidationEventInfo<TModel, string?> MustBeOfLengthOrBeNull(int length, Func<string>? formatMessage = null)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(length, 1);

            return eventInfo.Value is null || eventInfo.Value.Length >= length
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be {length} character(s).");
        }
        
        /// <summary>
        /// Adds a maximum string length check.
        /// </summary>
        /// <param name="length">The maximum string length to allow.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Length is less than 1.</exception>
        public ValidationEventInfo<TModel, string?> MustNotExceedLengthOrBeNull(int length, Func<string>? formatMessage = null)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(length, 1);

            return eventInfo.Value is null || eventInfo.Value.Length >= length
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be {length} character(s).");
        }
        
        /// <summary>
        /// Adds a regular expression pattern check.
        /// </summary>
        /// <param name="pattern">The regular expression to match.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public ValidationEventInfo<TModel, string?> MustMatchPatternOrBeNull(string pattern, Func<string>? formatMessage = null)
        {
            ArgumentNullException.ThrowIfNull(pattern);

            return eventInfo.Value is null || Regex.IsMatch(eventInfo.Value, pattern)
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must match pattern {pattern}.");
        }

        /// <summary>
        /// Adds a regular expression pattern check.
        /// </summary>
        /// <param name="regex">The regular expression to match.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public ValidationEventInfo<TModel, string?> MustMatchPatternOrBeNull(Regex regex, Func<string>? formatMessage = null)
        {
            ArgumentNullException.ThrowIfNull(regex);

            return eventInfo.Value is null || regex.IsMatch(eventInfo.Value)
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must match pattern {regex}.");
        }
    }
}