using System.Text;

namespace Vertical.Cli.IO;

internal sealed class ConfigurationErrorWriter
{
    private sealed class Data
    {
        public StringBuilder Buffer { get; } = new();
        public Dictionary<string, int> Notes = [];
    }

    private readonly Data _data;
    private readonly int _indent;
    private Dictionary<string, int> Notes => _data.Notes;
    private StringBuilder Buffer => _data.Buffer;

    public ConfigurationErrorWriter() : this(new(), 1)
    {
    }

    private ConfigurationErrorWriter(Data data, int indent)
    {
        _data = data;
        _indent = indent;
    }

    public ConfigurationErrorWriter Indent() => new(_data, _indent + 1);

    public int AddNote(string note)
    {
        if (Notes.TryGetValue(note, out var index))
            return index;

        Notes[note] = Notes.Count + 1;
        return Notes.Count;
    }
    
    public void AddMessage(string message)
    {
        if (_indent > 0)
        {
            Buffer.Append(' ', _indent * 2);
        }
        Buffer.AppendLine(message);
    }
    
    public void AddMessageGroup<T>(
        IEnumerable<T> items,
        Func<T, string> getContent)
    {
        foreach (var item in items)
        {
            AddMessage(getContent(item));
        }
    }
    
    public void AddMessageGroup<T>(
        IEnumerable<T> items,
        string title,
        Func<T, string> getContent)
    {
        var writeTitle = true;
        var builder = Indent();
        
        foreach (var item in items)
        {
            if (writeTitle)
            {
                AddMessage(title);
                writeTitle = false;
            }
            
            builder.AddMessage(getContent(item));
        }
    }
    
    public void AddMessageGroupWithNote<T>(
        IEnumerable<T> items,
        Func<int, string> getTitle,
        Func<T, string> getContent,
        string note)
    {
        var writeTitle = true;
        var builder = Indent();
        
        foreach (var item in items)
        {
            var noteId = AddNote(note);
            
            if (writeTitle)
            {
                AddMessage(getTitle(noteId));
                writeTitle = false;
            }
            
            builder.AddMessage(getContent(item));
        }
    }

    public void AddMessageGroup<TKey, TValue>(
        IEnumerable<IGrouping<TKey, TValue>> groupings,
        Func<TKey, string> getKeyContent,
        Func<TValue, string> getValueContent)
    {
        foreach (var grouping in groupings)
        {
            AddMessage(getKeyContent(grouping.Key));
            var builder = Indent();

            foreach (var value in grouping)
            {
                builder.AddMessage(getValueContent(value));
            }
        }
    }
    
    public void AddMessageGroupWithNote<TKey, TValue>(
        IEnumerable<IGrouping<TKey, TValue>> groupings,
        Func<TKey, int, string> getKeyContent,
        Func<TValue, string> getValueContent,
        string note)
    {
        foreach (var grouping in groupings)
        {
            var noteId = AddNote(note);
            AddMessage(getKeyContent(grouping.Key, noteId));
            var builder = Indent();

            foreach (var value in grouping)
            {
                builder.AddMessage(getValueContent(value));
            }
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (_data.Buffer.Length == 0)
            return string.Empty;
        
        var sb = new StringBuilder();

        sb.AppendLine("One or more errors configuration errors found.");
        sb.AppendLine("Errors:");
        sb.Append(Buffer);

        if (Notes.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Notes:");

            foreach (var (note, id) in Notes)
            {
                sb.Append($"  {id}) ");
                var lines = note.Split(Environment.NewLine);
                for (var c = 0; c < lines.Length; c++)
                {
                    if (c > 0) sb.Append("     ");
                    sb.AppendLine(lines[c]);
                }
            }
        }

        return sb.ToString();
    }
}