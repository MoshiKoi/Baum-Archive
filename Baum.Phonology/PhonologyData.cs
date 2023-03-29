namespace Baum.Phonology;

public class PhonologyData
{
    IEnumerable<Sound> _sounds;
    public PhonologyData(IEnumerable<Sound> sounds) => _sounds = sounds;
    public Sound GetSound(char symbol) => Enumerable.Single(_sounds.Where(sound => sound.Symbol == symbol));
    public Sound GetSound(IEnumerable<Feature> features)
        => Enumerable.Single(_sounds.Where(sound => sound.Features.SetEquals(features)));
    public IReadOnlySet<Sound> GetSounds(IReadOnlySet<Feature> includedFeatures, IReadOnlySet<Feature> excludedFeatures)
        => new HashSet<Sound>(_sounds.Where(sound => 
            includedFeatures.IsSubsetOf(sound.Features) &&
            !excludedFeatures.Intersect(sound.Features).Any()));
}