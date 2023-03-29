namespace Baum.Phonology;

public class PhonologyData
{
    IEnumerable<Sound> _sounds;
    public PhonologyData(IEnumerable<Sound> sounds) => _sounds = sounds;

    // TODO? Should this just return null?
    public Sound GetStartSound(string symbol)
        => _sounds.Where(sound => symbol.StartsWith(sound.Symbol))
            .MaxBy(sound => sound.Symbol.Length) ?? throw new Exception($"There is no sound for {symbol}");

    public Sound GetSound(IEnumerable<Feature> features)
        => Enumerable.Single(_sounds.Where(sound => sound.Features.SetEquals(features)));

    public IReadOnlySet<Sound> GetSounds(IReadOnlySet<Feature> includedFeatures, IReadOnlySet<Feature> excludedFeatures)
        => new HashSet<Sound>(_sounds.Where(sound =>
            includedFeatures.IsSubsetOf(sound.Features) &&
            !excludedFeatures.Intersect(sound.Features).Any()));
}