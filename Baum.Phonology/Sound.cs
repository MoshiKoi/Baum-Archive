namespace Baum.Phonology;

public record Feature(string Name);
public record Sound(char Symbol, IReadOnlySet<Feature> Features);