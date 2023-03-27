namespace Baum.Phonology;

public class IPARegexBuilder : MatchNodeVisitor<string>
{
    public static readonly IPARegexBuilder Instance = new();
    public string Visit(FeatureSetMatchNode node)
    {
        return '[' + String.Concat(
            from keyValuePair in IPA.IPADict
            where keyValuePair.Value.HasFlag(node.Included) && ((keyValuePair.Value & node.Excluded) == Features.None)
            select keyValuePair.Key) + ']';
    }
}