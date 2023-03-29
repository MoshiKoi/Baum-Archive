namespace Baum.Phonology.Notation;

public abstract record MatchNode
{
    public abstract T Accept<T>(MatchNodeVisitor<T> visitor);
}

public record FeatureSetMatchNode(IReadOnlySet<Feature> Included, IReadOnlySet<Feature> Excluded) : MatchNode
{
    public override T Accept<T>(MatchNodeVisitor<T> visitor) => visitor.Visit(this);
}

public record SoundMatchNode(IReadOnlySet<Feature> Features) : MatchNode
{
    public override T Accept<T>(MatchNodeVisitor<T> visitor) => visitor.Visit(this);
}