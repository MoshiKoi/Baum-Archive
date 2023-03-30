using Baum.Rewrite;
using Baum.Phonology.Notation;

namespace Baum.Phonology.Notation;

class SoundChangeRewriteParser : IMatchNodeVisitor<IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>>>
{
    public IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>> Visit(FeatureSetMatchNode matchNode)
        => new SoundMatchRewriteParser(featureSet
            => featureSet.IsSupersetOf(matchNode.Included)
            && !featureSet.Intersect(matchNode.Excluded).Any());

    public IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>> Visit(SoundMatchNode matchNode)
        => new SoundMatchRewriteParser(featureSet => featureSet.SetEquals(matchNode.Features));
}

class SoundMatchRewriteParser : IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>>
{
    Predicate<IReadOnlySet<Feature>> Match;
    public SoundMatchRewriteParser(Predicate<IReadOnlySet<Feature>> match) => Match = match;

    public IRewriter<IReadOnlySet<Feature>> Visit(FeatureSetMatchNode replaceNode)
    {
        throw new NotImplementedException();
    }

    public IRewriter<IReadOnlySet<Feature>> Visit(SoundMatchNode replaceNode)
        => new MatchRewriter<IReadOnlySet<Feature>>(Match, replaceNode.Features);
}