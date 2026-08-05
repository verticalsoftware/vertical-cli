namespace Vertical.Cli.Validation;

/// <summary>
/// Defines validation functions for file system objects.
/// </summary>
public static class FileSystemValidationExtensions
{
    extension<TModel>(IValidationEventInfo<TModel, FileInfo> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a value check that verifies the file exists.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, FileInfo> MustExist(Func<string>? formatMessage = null)
        {
            return eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? "file not found.");
        }
        
        /// <summary>
        /// Adds a value check that verifies the file does not exist.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, FileInfo> CannotExist(Func<string>? formatMessage = null)
        {
            return !eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.ToString() ?? "file already exists.");
        }
    }
    
    extension<TModel>(IValidationEventInfo<TModel, FileInfo?> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a value check that verifies the file exists.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, FileInfo?> MustExistOrBeNull(Func<string>? formatMessage = null)
        {
            return eventInfo.Value is null || eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? "file not found.");
        }
        
        /// <summary>
        /// Adds a value check that verifies the file does not exist.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, FileInfo?> CannotExistOrBeNull(Func<string>? formatMessage = null)
        {
            return eventInfo.Value is null || !eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.ToString() ?? "file already exists.");
        }
    }
    
    extension<TModel>(IValidationEventInfo<TModel, DirectoryInfo> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a value check that verifies the directory exists.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, DirectoryInfo> MustExist(Func<string>? formatMessage = null)
        {
            return eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? "file not found.");
        }
        
        /// <summary>
        /// Adds a value check that verifies the directory does not exist.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, DirectoryInfo> CannotExist(Func<string>? formatMessage = null)
        {
            return !eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.ToString() ?? "file already exists.");
        }
    }
    
    extension<TModel>(IValidationEventInfo<TModel, DirectoryInfo?> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a value check that verifies the directory exists.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, DirectoryInfo?> MustExistOrBeNull(Func<string>? formatMessage = null)
        {
            return eventInfo.Value is null || eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.Invoke() ?? "file not found.");
        }
        
        /// <summary>
        /// Adds a value check that verifies the directory does not exist.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public IValidationEventInfo<TModel, DirectoryInfo?> CannotExistOrBeNull(Func<string>? formatMessage = null)
        {
            return eventInfo.Value is null || !eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.ToString() ?? "file already exists.");
        }
    }
}