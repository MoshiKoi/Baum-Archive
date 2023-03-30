using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Baum.Rewrite;


public class SequenceRewriter<T> : IRewriter<T>, IEnumerable<IRewriter<T>>
{
    List<IRewriter<T>> Rewriters = new();

    #region IRewriter<T>

    // TODO: Consider: If Rewriters is empty, should it throw, return an empty result, or return an unchanged result
    public IEnumerable<RewritePair<T>> Rewrite(IEnumerable<T> sequence, int startPosition)
    {
        var rewritePairs = Rewriters.First().Rewrite(sequence, startPosition);

        foreach (var rewriter in Rewriters.Skip(1))
        {
            rewritePairs = rewritePairs.SelectMany(pair
                => rewriter.Rewrite(sequence, pair.RewritePosition)
                    .Select(nextPair => new RewritePair<T>
                    {
                        Rewrite = pair.Rewrite.Concat(nextPair.Rewrite),
                        RewritePosition = nextPair.RewritePosition
                    }));
        }

        return rewritePairs;
    }

    #endregion

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