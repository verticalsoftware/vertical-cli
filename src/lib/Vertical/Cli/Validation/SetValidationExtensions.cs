namespace Vertical.Cli.Validation;

/// <summary>
/// Defines validation function based on set values.
/// </summary>
public static class SetValidationExtensions
{
    extension<TModel, TValue>(IValidationEventInfo<TModel, TValue> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a check that requires an argument value to be contained in a set of values.
        /// </summary>
        /// <param name="set">A hashset that contains the allowed values.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Length is less than 1.</exception>
        public IValidationEventInfo<TModel, TValue> MustBeOneOf(
            HashSet<TValue> set,
            Func<string>? formatMessage = null)
        {
            ArgumentNullException.ThrowIfNull(set);

            return set.Contains(eventInfo.Value)
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be one of [{string.Join(", ", set)}]");
        }
        
        /// <summary>
        /// Adds a check that requires an argument value to not be contained in a set of values.
        /// </summary>
        /// <param name="set">A hashset that contains the unacceptable values.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Length is less than 1.</exception>
        public IValidationEventInfo<TModel, TValue> CannotBeOneOf(
            HashSet<TValue> set,
            Func<string>? formatMessage = null)
        {
            ArgumentNullException.ThrowIfNull(set);

            return !set.Contains(eventInfo.Value)
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be one of [{string.Join(", ", set)}]");
        }
    }
    
    extension<TModel, TValue>(IValidationEventInfo<TModel, TValue?> eventInfo) 
        where TModel : class
        where TValue : struct
    {
        /// <summary>
        /// Adds a check that requires an argument value to be contained in a set of values.
        /// </summary>
        /// <param name="set">A hashset that contains the allowed values.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Length is less than 1.</exception>
        public IValidationEventInfo<TModel, TValue?> MustBeOneOfOrBeNull(
            HashSet<TValue?> set,
            Func<string>? formatMessage = null)
        {
            ArgumentNullException.ThrowIfNull(set);

            return set.Contains(eventInfo.Value)
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be one of [{string.Join(", ", set)}]");
        }
        
        /// <summary>
        /// Adds a check that requires an argument value to not be contained in a set of values.
        /// </summary>
        /// <param name="set">A hashset that contains the unacceptable values.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Length is less than 1.</exception>
        public IValidationEventInfo<TModel, TValue?> CannotBeOneOfOrBeNull(
            HashSet<TValue?> set,
            Func<string>? formatMessage = null)
        {
            ArgumentNullException.ThrowIfNull(set);

            return !set.Contains(eventInfo.Value)
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be one of [{string.Join(", ", set)}]");
        }
    }
}