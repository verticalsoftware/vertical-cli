using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Defines comparers for Cli object names.
/// </summary>
public static class IdentifierComparer
{
    /// <summary>
    /// Defines the default implementation.
    /// </summary>
    public static readonly IComparer<ICliSymbol> Default = new DefaultImpl();
    
    /// <summary>
    /// Defines the sorted implementation.
    /// </summary>
    public static readonly IComparer<ICliSymbol> Sorted = new SortedImpl();
    
    private sealed class SortedImpl : IComparer<ICliSymbol>
    {
        public int Compare(ICliSymbol? x, ICliSymbol? y)
            {
                if (ReferenceEquals(x, y))
                    return 0;
        
                if (ReferenceEquals(x, null))
                    return -1;
        
                if (ReferenceEquals(y, null))
                    return 1;
        
                var xSpan = GetSpan(x);
                var ySpan = GetSpan(y);
        
                var i = 0;
                var charComparer = Comparer<char>.Default;
                
                for (;; i++)
                {
                    if (i == xSpan.Length || i == ySpan.Length)
                        break;
        
                    var c = charComparer.Compare(xSpan[i], ySpan[i]);
        
                    if (c != 0)
                        return c;
                }
        
                return 0;
            }
        
            private static ReadOnlySpan<char> GetSpan(ICliSymbol obj)
            {
                var name = obj.Names.Length != 0 ? obj.Names[0] : obj.PrimaryIdentifier;
                var span = name.AsSpan();
        
                while (span.Length > 0 && !char.IsLetterOrDigit(span[0]))
                    span = span[1..];
                
                return span;
            }
    }

    private sealed class DefaultImpl : IComparer<ICliSymbol>
    {
        public int Compare(ICliSymbol? x, ICliSymbol? y)
        {
            if (ReferenceEquals(x, y))
                return 0;
        
            if (ReferenceEquals(x, null))
                return -1;
        
            if (ReferenceEquals(y, null))
                return 1;

            return Comparer<int>.Default.Compare(x.Index, y.Index);
        }
    }
}