using System.Diagnostics.CodeAnalysis;

namespace Baum.Rewrite;

public class MatchRewriter<T> : IRewriter<T>
{
    Predicate<T> Match;
    Func<T, T> Replace;

    public MatchRewriter(T match) : this(x => object.Equals(x, match)) { }
    public MatchRewriter(Predicate<T> match) : this(match, x => x) { }
    public MatchRewriter(T match, T replacement) : this(x => object.Equals(x, match), x => replacement) { }
    public MatchRewriter(Predicate<T> match, T replacement) : this(match, x => replacement) { }
    public MatchRewriter(Predicate<T> match, Func<T, T> replace)
        => (Match, Replace) = (match, replace);


    public IEnumerable<RewritePair<T>> Rewrite(IEnumerable<T> sequence, int startPosition)
    {
        var element = sequence.ElementAtOrDefault(startPosition);
        if (element is not null && Match(element))
        {
            return new RewritePair<T>[] {
                new RewritePair<T> {
                    Rewrite = new [] { Replace(element) },
                    RewritePosition = startPosition + 1
                }
            };
        }
        else
        {
            return Enumerable.Empty<RewritePair<T>>();
        }
    }
}