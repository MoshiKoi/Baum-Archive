using Baum.Rewrite;
using Baum.Phonology.Notation;

namespace Baum.Phonology;

class SoundChangeRewriteParser : IMatchNodeVisitor<IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>>>
{
    public IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>> Visit(FeatureSetMatchNode matchNode)
    {
        throw new NotImplementedException();
    }

    public IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>> Visit(SoundMatchNode matchNode)
        => new SoundMatchRewriteParser(matchNode);
}

class SoundMatchRewriteParser : IMatchNodeVisitor<IRewriter<IReadOnlySet<Feature>>>
{
    SoundMatchNode _matchNnode;
    public SoundMatchRewriteParser(SoundMatchNode matchNode) => _matchNnode = matchNode;

    public IRewriter<IReadOnlySet<Feature>> Visit(FeatureSetMatchNode replaceNode)
    {
        throw new NotImplementedException();
    }

    public IRewriter<IReadOnlySet<Feature>> Visit(SoundMatchNode replaceNode)
        => new MatchRewriter<IReadOnlySet<Feature>>(
            featureSet => featureSet.SetEquals(_matchNnode.Features),
            replaceNode.Features);
}