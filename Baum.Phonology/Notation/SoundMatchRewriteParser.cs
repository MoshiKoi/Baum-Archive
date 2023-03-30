using Baum.Rewrite;
using Baum.Phonology.Notation;

namespace Baum.Phonology.Notation;

class SoundMatchRewriteMatchParser : IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>>
{
    public IRewriter<IReadOnlySet<Feature>> Visit(FeatureSetMatchNode matchNode)
        => new MatchRewriter<IReadOnlySet<Feature>>(featureSet
            => featureSet.IsSupersetOf(matchNode.Included)
            && !featureSet.Intersect(matchNode.Excluded).Any());

    public IRewriter<IReadOnlySet<Feature>> Visit(SoundMatchNode matchNode)
        => new MatchRewriter<IReadOnlySet<Feature>>(featureSet => featureSet.SetEquals(matchNode.Features));
}