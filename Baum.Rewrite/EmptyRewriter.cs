namespace Baum.Rewrite;

public class EmptyRewriter<T> : IRewriter<T>
{
    IEnumerable<T> Insertion;

    public EmptyRewriter(IEnumerable<T> insertion) => Insertion = insertion;

    public IEnumerable<RewritePair<T>> Rewrite(IEnumerable<T> sequence, int startPosition)
    {
        return new RewritePair<T>[]
        {
            new RewritePair<T>
            {
                Rewrite = Insertion,
                RewritePosition = startPosition
            }
        };
    }
}