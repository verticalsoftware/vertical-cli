namespace OptionsGroups;

public record Model(
    int LogLevel,
    bool Debug,
    string Host,
    int Port,
    string Database,
    string UserId,
    string Password,
    string Command,
    string[] Properties);