namespace Vertical.Cli.Validation;

public static class ComparableValidationExtensions
{
    extension<TModel, TValue>(IValidationEventInfo<TModel, TValue> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a check that verifies an argument is less than the provided value.
        /// </summary>
        /// <param name="value">The upper limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue> MustBeLessThan(TValue value,
            IComparer<TValue>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return (comparer ?? Comparer<TValue>.Default).Compare(eventInfo.Value, value) < 0
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be less than {value}.");
        }
        
        /// <summary>
        /// Adds a check that verifies an argument is less than or equal to the provided value.
        /// </summary>
        /// <param name="value">The upper limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue> MustBeLessOrEqualTo(TValue value,
            IComparer<TValue>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return (comparer ?? Comparer<TValue>.Default).Compare(eventInfo.Value, value) <= 0
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be less than {value}.");
        }
        
        /// <summary>
        /// Adds a check that verifies an argument is greater than the provided value.
        /// </summary>
        /// <param name="value">The lower limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue> MustBeGreaterThan(TValue value,
            IComparer<TValue>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return (comparer ?? Comparer<TValue>.Default).Compare(eventInfo.Value, value) > 0
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be greater than {value}.");
        }
        
        /// <summary>
        /// Adds a check that verifies an argument is greater than or equal to the provided value.
        /// </summary>
        /// <param name="value">The lower limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue> MustBeGreaterOrEqualTo(TValue value,
            IComparer<TValue>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return (comparer ?? Comparer<TValue>.Default).Compare(eventInfo.Value, value) >= 0
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be greater than {value}.");
        }

        /// <summary>
        /// Combines the lower and upper value checks into an inclusive range.
        /// </summary>
        /// <param name="lowerValue">The lower limit value.</param>
        /// <param name="upperValue">The upper limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue> MustBeInTheInclusiveRangeOf(
            TValue lowerValue,
            TValue upperValue,
            IComparer<TValue>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return eventInfo
                .MustBeGreaterOrEqualTo(lowerValue, comparer, formatMessage)
                .MustBeLessOrEqualTo(upperValue, comparer, formatMessage);
        }
        
        /// <summary>
        /// Combines the lower and upper value checks into an exclusive range.
        /// </summary>
        /// <param name="lowerValue">The lower limit value.</param>
        /// <param name="upperValue">The upper limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue> MustBeInTheExclusiveRangeOf(
            TValue lowerValue,
            TValue upperValue,
            IComparer<TValue>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return eventInfo
                .MustBeGreaterThan(lowerValue, comparer, formatMessage)
                .MustBeLessThan(upperValue, comparer, formatMessage);
        }
    }
    
    extension<TModel, TValue>(IValidationEventInfo<TModel, TValue?> eventInfo) 
        where TModel : class
        where TValue : struct
    {
        /// <summary>
        /// Adds a check that verifies an argument is less than the provided value.
        /// </summary>
        /// <param name="value">The upper limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue?> MustBeLessThanOrBeNull(
            TValue? value,
            IComparer<TValue?>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return !eventInfo.Value.HasValue || (comparer ?? Comparer<TValue?>.Default).Compare(eventInfo.Value, value) < 0
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be less than {value}.");
        }
        
        /// <summary>
        /// Adds a check that verifies an argument is less than or equal to the provided value.
        /// </summary>
        /// <param name="value">The upper limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue?> MustBeLessOrEqualToOrBeNull(
            TValue? value,
            IComparer<TValue?>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return !eventInfo.Value.HasValue || (comparer ?? Comparer<TValue?>.Default).Compare(eventInfo.Value, value) <= 0
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be less than {value}.");
        }
        
        /// <summary>
        /// Adds a check that verifies an argument is greater than the provided value.
        /// </summary>
        /// <param name="value">The lower limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue?> MustBeGreaterThanOrBeNull(
            TValue? value,
            IComparer<TValue?>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return !eventInfo.Value.HasValue || (comparer ?? Comparer<TValue?>.Default).Compare(eventInfo.Value, value) > 0
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be greater than {value}.");
        }
        
        /// <summary>
        /// Adds a check that verifies an argument is greater than or equal to the provided value.
        /// </summary>
        /// <param name="value">The lower limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue?> MustBeGreaterOrEqualToOrBeNull(TValue? value,
            IComparer<TValue?>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return !eventInfo.Value.HasValue || (comparer ?? Comparer<TValue?>.Default).Compare(eventInfo.Value, value) >= 0
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? $"value must be greater than {value}.");
        }

        /// <summary>
        /// Combines the lower and upper value checks into an inclusive range.
        /// </summary>
        /// <param name="lowerValue">The lower limit value.</param>
        /// <param name="upperValue">The upper limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue?> MustBeInTheInclusiveRangeOfOrBeNull(
            TValue? lowerValue,
            TValue? upperValue,
            IComparer<TValue?>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return eventInfo
                .MustBeGreaterOrEqualToOrBeNull(lowerValue, comparer, formatMessage)
                .MustBeLessOrEqualToOrBeNull(upperValue, comparer, formatMessage);
        }
        
        /// <summary>
        /// Combines the lower and upper value checks into an exclusive range.
        /// </summary>
        /// <param name="lowerValue">The lower limit value.</param>
        /// <param name="upperValue">The upper limit value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, TValue?> MustBeInTheExclusiveRangeOfOrBeNull(
            TValue? lowerValue,
            TValue? upperValue,
            IComparer<TValue?>? comparer = null,
            Func<string>? formatMessage = null)
        {
            return eventInfo
                .MustBeGreaterThanOrBeNull(lowerValue, comparer, formatMessage)
                .MustBeLessThanOrBeNull<TModel, TValue>(upperValue, comparer, formatMessage);
        }
    }
}