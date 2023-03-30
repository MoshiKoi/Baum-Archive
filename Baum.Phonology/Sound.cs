namespace Baum.Phonology;

public sealed record Feature(string Name);
public sealed record Sound(string Symbol, IReadOnlySet<Feature> Features) : IEquatable<Sound>
{
    public override int GetHashCode() => (Symbol, Features).GetHashCode();
    public bool Equals(Sound? other)
        => other is not null
        && Symbol == other.Symbol
        && Features.SetEquals(other.Features);
}