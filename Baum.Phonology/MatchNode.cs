namespace Baum.Phonology;

//
//    root               changeroot
//     |             ->     |
//    [+consonant]         [+voice]

public abstract record MatchNode
{
    public abstract T Accept<T>(MatchNodeVisitor<T> visitor);
}

public record FeatureSetMatchNode(Features Included, Features Excluded) : MatchNode
{
    public override T Accept<T>(MatchNodeVisitor<T> visitor) => visitor.Visit(this);
    public static FeatureSetMatchNode ExactMatch(Features sound) => new(sound, ~sound);
}