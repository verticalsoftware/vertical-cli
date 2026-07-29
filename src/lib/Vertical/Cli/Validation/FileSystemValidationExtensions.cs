namespace Vertical.Cli.Validation;

public static class FileSystemValidationExtensions
{
    extension<TModel>(ValidationEventInfo<TModel, FileInfo> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a value check that verifies the file exists.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public ValidationEventInfo<TModel, FileInfo> MustExist(Func<string>? formatMessage = null)
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
        public ValidationEventInfo<TModel, FileInfo> CannotExist(Func<string>? formatMessage = null)
        {
            return !eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.ToString() ?? "file already exists.");
        }
    }
    
    extension<TModel>(ValidationEventInfo<TModel, FileInfo?> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a value check that verifies the file exists.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public ValidationEventInfo<TModel, FileInfo?> MustExistOrBeNull(Func<string>? formatMessage = null)
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
        public ValidationEventInfo<TModel, FileInfo?> CannotExistOrBeNull(Func<string>? formatMessage = null)
        {
            return eventInfo.Value is null || !eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.ToString() ?? "file already exists.");
        }
    }
    
    extension<TModel>(ValidationEventInfo<TModel, DirectoryInfo> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a value check that verifies the directory exists.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public ValidationEventInfo<TModel, DirectoryInfo> MustExist(Func<string>? formatMessage = null)
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
        public ValidationEventInfo<TModel, DirectoryInfo> CannotExist(Func<string>? formatMessage = null)
        {
            return !eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.ToString() ?? "file already exists.");
        }
    }
    
    extension<TModel>(ValidationEventInfo<TModel, DirectoryInfo?> eventInfo) where TModel : class
    {
        /// <summary>
        /// Adds a value check that verifies the directory exists.
        /// </summary>
        /// <param name="formatMessage">
        /// An optional method used to format the message displayed to the user.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public ValidationEventInfo<TModel, DirectoryInfo?> MustExistOrBeNull(Func<string>? formatMessage = null)
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
        public ValidationEventInfo<TModel, DirectoryInfo?> CannotExistOrBeNull(Func<string>? formatMessage = null)
        {
            return eventInfo.Value is null || !eventInfo.Value.Exists
                ? eventInfo.OK
                : eventInfo.Error(formatMessage?.ToString() ?? "file already exists.");
        }
    }
}