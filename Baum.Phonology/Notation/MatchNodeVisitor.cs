namespace Baum.Phonology.Notation;

public interface MatchNodeVisitor<T>
{
    T Visit(FeatureSetMatchNode node);
    T Visit(SoundMatchNode node);
}