namespace Vertical.Cli.Validation;

/// <summary>
/// Extensions for System.IO types
/// </summary>
public static class FileSystemValidationExtensions
{
    /// <summary>
    /// Adds a rule that the provided file must exist.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, FileInfo> Exists<TModel>(
        this ValidationBuilder<TModel, FileInfo> instance,
        string? message = null) where TModel : class
    {
        return instance.Must((_, fsObj) => fsObj.Exists,
            message ?? "file not found");
    }
    
    /// <summary>
    /// Adds a rule that the provided directory must exist.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, DirectoryInfo> Exists<TModel>(
        this ValidationBuilder<TModel, DirectoryInfo> instance,
        string? message = null) where TModel : class
    {
        return instance.Must((_, fsObj) => fsObj.Exists,
            message ?? "directory not found");
    }
    
    /// <summary>
    /// Adds a rule that the provided file must not exist.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, FileInfo> NotExists<TModel>(
        this ValidationBuilder<TModel, FileInfo> instance,
        string? message = null) where TModel : class
    {
        return instance.Must((_, fsObj) => !fsObj.Exists,
            message ?? "file exists");
    }
    
    /// <summary>
    /// Adds a rule that the provided directory must not exist.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, DirectoryInfo> NotExists<TModel>(
        this ValidationBuilder<TModel, DirectoryInfo> instance,
        string? message = null) where TModel : class
    {
        return instance.Must((_, fsObj) => !fsObj.Exists,
            message ?? "directory exists");
    }
    
    /// <summary>
    /// Adds a rule that the provided file must exist.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, FileInfo?> ExistsOrNull<TModel>(
        this ValidationBuilder<TModel, FileInfo?> instance,
        string? message = null) where TModel : class
    {
        return instance.Must((_, fsObj) => fsObj is null || fsObj.Exists,
            message ?? "file not found");
    }
    
    /// <summary>
    /// Adds a rule that the provided directory must exist.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, DirectoryInfo?> ExistsOrNull<TModel>(
        this ValidationBuilder<TModel, DirectoryInfo?> instance,
        string? message = null) where TModel : class
    {
        return instance.Must((_, fsObj) => fsObj is null || fsObj.Exists,
            message ?? "directory not found");
    }
    
    /// <summary>
    /// Adds a rule that the provided file must not exist.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, FileInfo?> NotExistsOrNull<TModel>(
        this ValidationBuilder<TModel, FileInfo?> instance,
        string? message = null) where TModel : class
    {
        return instance.Must((_, fsObj) => fsObj is null || !fsObj.Exists,
            message ?? "file exists");
    }
    
    /// <summary>
    /// Adds a rule that the provided directory must not exist.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, DirectoryInfo?> NotExistsOrNull<TModel>(
        this ValidationBuilder<TModel, DirectoryInfo?> instance,
        string? message = null) where TModel : class
    {
        return instance.Must((_, fsObj) => fsObj is null || !fsObj.Exists,
            message ?? "directory exists");
    }
}