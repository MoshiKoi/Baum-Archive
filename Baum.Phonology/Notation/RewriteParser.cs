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

    public IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>> Visit(MatchListNode matchNode)
        => new MatchListRewriteParser(matchNode.Nodes);
}

class SoundMatchRewriteParser : IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>>
{
    Predicate<IReadOnlySet<Feature>> Match;
    public SoundMatchRewriteParser(Predicate<IReadOnlySet<Feature>> match) => Match = match;

    public IRewriter<IReadOnlySet<Feature>> Visit(FeatureSetMatchNode replaceNode)
        => new MatchRewriter<IReadOnlySet<Feature>>(
            Match,
            match => new[] { new HashSet<Feature>(match.Except(replaceNode.Excluded).Union(replaceNode.Included)) });

    public IRewriter<IReadOnlySet<Feature>> Visit(SoundMatchNode replaceNode)
        => new MatchRewriter<IReadOnlySet<Feature>>(Match, new[] { replaceNode.Features });

    public IRewriter<IReadOnlySet<Feature>> Visit(EmptyNode node)
        => new MatchRewriter<IReadOnlySet<Feature>>(Match, Enumerable.Empty<IReadOnlySet<Feature>>());

    public IRewriter<IReadOnlySet<Feature>> Visit(MatchListNode replaceNode)
        // a > {b,c} makes no sense
        => throw new Exception("Cannot decide between replacements in list");
}

class MatchListRewriteParser : IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>>
{
    List<MatchNode> MatchNodes { get; set; }

    public MatchListRewriteParser(List<MatchNode> matchNodes) => MatchNodes = matchNodes;

    public IRewriter<IReadOnlySet<Feature>> Visit(FeatureSetMatchNode replaceNode)
        => new AlternativeRewriter<IReadOnlySet<Feature>>(
            MatchNodes.Select(matchNode => matchNode.Accept(new SoundChangeRewriteParser()).Visit(replaceNode)));

    public IRewriter<IReadOnlySet<Feature>> Visit(SoundMatchNode replaceNode)
        => new AlternativeRewriter<IReadOnlySet<Feature>>(
            MatchNodes.Select(matchNode => matchNode.Accept(new SoundChangeRewriteParser()).Visit(replaceNode)));

    public IRewriter<IReadOnlySet<Feature>> Visit(MatchListNode replaceNode)
        => new AlternativeRewriter<IReadOnlySet<Feature>>(
            Enumerable.Zip(MatchNodes, replaceNode.Nodes)
                .Select(pair => pair.Second.Accept(pair.First.Accept(new SoundChangeRewriteParser()))));

    public IRewriter<IReadOnlySet<Feature>> Visit(EmptyNode replaceNode)
        => new AlternativeRewriter<IReadOnlySet<Feature>>(
            MatchNodes.Select(matchNode => matchNode.Accept(new SoundChangeRewriteParser()).Visit(replaceNode)));
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

    public IRewriter<IReadOnlySet<Feature>> Visit(MatchListNode node)
        => throw new Exception("Cannot decide between replacements in list");
}