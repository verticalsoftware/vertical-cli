namespace BasicSetup;

public enum Compression
{
    None,
    GZip
}

public class FileCopyParameters
{
    public FileCopyParameters(    
        FileInfo source,
        FileInfo dest,
        Compression compression,
        bool overwrite)
    {
        Source = source;
        Dest = dest;
        Compression = compression;
        Overwrite = overwrite;
    }

    public FileInfo Source { get; }

    public FileInfo Dest { get; }

    public Compression Compression { get; }

    public bool Overwrite { get; }
}