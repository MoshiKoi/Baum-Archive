namespace Baum.Rewrite;

public class EndRewriter<T> : IRewriter<T>
{
    IEnumerable<T> Insertion;

    public EndRewriter(IEnumerable<T> insertion) => Insertion = insertion;

    public IEnumerable<RewritePair<T>> Rewrite(IEnumerable<T> sequence, int startPosition)
    {
        if (startPosition == sequence.Count())
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
        else {
            return Enumerable.Empty<RewritePair<T>>();
        }
    }
}