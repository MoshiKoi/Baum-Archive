namespace Baum.Phonology.Notation;

public class IPARegexBuilder : MatchNodeVisitor<string>
{
    PhonologyData PhonologyData { get; }

    public IPARegexBuilder(PhonologyData data) => PhonologyData = data;

    public string Visit(FeatureSetMatchNode node)
    {
        var symbols = PhonologyData
                .GetSounds(node.Included, node.Excluded)
                .Select(sound => sound.Symbol);

        return '[' + String.Concat(symbols) + ']';
    }

    public string Visit(SoundMatchNode node) => PhonologyData.GetSound(node.Features).Symbol.ToString();
}