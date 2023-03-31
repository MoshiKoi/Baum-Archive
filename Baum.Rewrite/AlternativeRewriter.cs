using System.Collections;

namespace Baum.Rewrite;

public class AlternativeRewriter<T> : IRewriter<T>, IEnumerable<IRewriter<T>>
{
    List<IRewriter<T>> Rewriters { get; set; }

    public AlternativeRewriter() : this(new List<IRewriter<T>>()) { }
    public AlternativeRewriter(IEnumerable<IRewriter<T>> rewriters) => Rewriters = rewriters.ToList();

    public IEnumerable<RewritePair<T>> Rewrite(IEnumerable<T> sequence, int startPosition)
        => Rewriters.SelectMany(rewriter => rewriter.Rewrite(sequence, startPosition));

    #region IEnumerable<IRewriter<T>>

    public IEnumerator<IRewriter<T>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    #endregion

    public void Add(IRewriter<T> rewriter) => Rewriters.Add(rewriter);
}