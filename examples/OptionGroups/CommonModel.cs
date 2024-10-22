namespace OptionsGroups;

public class CommonModel
{
    public int LogLevel { get; init; }
    public bool Debug { get; init; }
    public string? Host { get; init; }
    public int Port { get; init; }
    public string? Database { get; init; }
    public string? UserId { get; init; }
    public string? Password { get; init; }
    public string? Command { get; init; }
    public string[]? Properties { get; init; }
}