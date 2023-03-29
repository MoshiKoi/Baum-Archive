namespace Baum.Phonology;

public record Feature(string Name);
public record Sound(string Symbol, IReadOnlySet<Feature> Features);