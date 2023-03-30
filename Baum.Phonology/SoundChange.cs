using Baum.Phonology.Notation;

namespace Baum.Phonology;

public class SoundChange
{
    class MatchVisitor : MatchNodeVisitor<bool>
    {
        IReadOnlySet<Feature> _features;
        public MatchVisitor(IReadOnlySet<Feature> features) => _features = features;
        public bool Visit(FeatureSetMatchNode node)
            => _features.IsSupersetOf(node.Included) && !_features.Intersect(node.Excluded).Any();
        public bool Visit(SoundMatchNode node) => _features.SetEquals(node.Features);
    }

    class ReplaceVisitor : MatchNodeVisitor<IReadOnlySet<Feature>>
    {
        IReadOnlySet<Feature> _features;
        public ReplaceVisitor(IReadOnlySet<Feature> features) => _features = features;
        public IReadOnlySet<Feature> Visit(FeatureSetMatchNode node)
            => new HashSet<Feature>(_features.Except(node.Excluded).Union(node.Included));

        public IReadOnlySet<Feature> Visit(SoundMatchNode node) => node.Features;
    }

    public static bool TryApply(string initial, string rule, PhonologyData data, out string after)
    {
        try
        {
            var node = NotationParser.Parse(rule, data);
            var change = new SoundChange
            {
                PhonologyData = data,
                MatchNode = node.Match,
                Replacement = node.Replace,
            };

            after = change.Apply(initial);
            return true;
        }
        catch (Exception) // TODO: Specialize exception
        {
            after = initial;
            return false;
        }
    }

    public required PhonologyData PhonologyData { get; set; }
    public required MatchNode MatchNode { get; set; }
    public required MatchNode Replacement { get; set; }
    public string Apply(string str)
    {
        return string.Concat(new Tokenization(str, PhonologyData)
            .Select(sound => ((SoundToken)sound).Features)
            .Select(features => MatchNode.Accept(new MatchVisitor(features))
                ? Replacement.Accept(new ReplaceVisitor(features))
                : features)
            .Select(PhonologyData.GetSound)
            .Select(sound => sound.Symbol));
    }
}