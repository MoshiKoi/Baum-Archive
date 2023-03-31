using Baum.Rewrite;
using Baum.Phonology.Notation;

namespace Baum.Phonology.Notation;

// Only does matching, not replacing
class SoundMatchRewriteMatchParser : IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>>
{
    public IRewriter<IReadOnlySet<Feature>> Visit(FeatureSetMatchNode matchNode)
        => new MatchRewriter<IReadOnlySet<Feature>>(featureSet
            => featureSet.IsSupersetOf(matchNode.Included)
            && !featureSet.Intersect(matchNode.Excluded).Any());

    public IRewriter<IReadOnlySet<Feature>> Visit(SoundMatchNode matchNode)
        => new MatchRewriter<IReadOnlySet<Feature>>(featureSet => featureSet.SetEquals(matchNode.Features));

    public IRewriter<IReadOnlySet<Feature>> Visit(EmptyNode node)
        => new EmptyRewriter<IReadOnlySet<Feature>>(Enumerable.Empty<IReadOnlySet<Feature>>());

    public IRewriter<IReadOnlySet<Feature>> Visit(MatchListNode node)
        => new AlternativeRewriter<IReadOnlySet<Feature>>(node.Nodes.Select(node => node.Accept(this)));

    public IRewriter<IReadOnlySet<Feature>> Visit(EndMatchNode node)
        => new EndRewriter<IReadOnlySet<Feature>>(Enumerable.Empty<IReadOnlySet<Feature>>());
}