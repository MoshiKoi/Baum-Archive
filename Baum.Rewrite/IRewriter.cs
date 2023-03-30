using System.Diagnostics.CodeAnalysis;

namespace Baum.Rewrite;

public struct RewritePair<T>
{
    public required IEnumerable<T> Rewrite;
    public required int RewritePosition;
}

public interface IRewriter<T>
{
    IEnumerable<RewritePair<T>> Rewrite(IEnumerable<T> sequence, int startPosition);
}
