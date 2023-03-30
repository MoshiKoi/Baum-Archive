namespace Baum.Phonology.Notation;

public abstract record MatchNode
{
    public abstract T Accept<T>(IMatchNodeVisitor<T> visitor);
}

public record FeatureSetMatchNode(IReadOnlySet<Feature> Included, IReadOnlySet<Feature> Excluded) : MatchNode
{
    public override T Accept<T>(IMatchNodeVisitor<T> visitor) => visitor.Visit(this);
}

public record SoundMatchNode(IReadOnlySet<Feature> Features) : MatchNode
{
    public override T Accept<T>(IMatchNodeVisitor<T> visitor) => visitor.Visit(this);
}

public record EmptyNode : MatchNode
{
    public override T Accept<T>(IMatchNodeVisitor<T> visitor) => visitor.Visit(this);
}