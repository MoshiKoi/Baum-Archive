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

    public IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>> Visit(EmptyNode matchNode)
        => new EmptyMatchRewriteParser();
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
        => new MatchRewriter<IReadOnlySet<Feature>>(Match, new[] { replaceNode.Features });

    public IRewriter<IReadOnlySet<Feature>> Visit(EmptyNode node)
        => new MatchRewriter<IReadOnlySet<Feature>>(Match, Enumerable.Empty<IReadOnlySet<Feature>>());
}

class EmptyMatchRewriteParser : IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>>
{
    public IRewriter<IReadOnlySet<Feature>> Visit(FeatureSetMatchNode replaceNode)
        // {} > [+voiced] makes little sense
        => throw new Exception("Cannot add or subtract features from the empty symbol");

    public IRewriter<IReadOnlySet<Feature>> Visit(SoundMatchNode replaceNode)
        => new EmptyRewriter<IReadOnlySet<Feature>>(new[] { replaceNode.Features });

    public IRewriter<IReadOnlySet<Feature>> Visit(EmptyNode replaceNode)
        // {} > {} also makes no sense
        => throw new Exception("Cannot replace nothing with nothing");
}