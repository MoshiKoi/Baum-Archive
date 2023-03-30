namespace Baum.Phonology.Notation;

public interface IMatchNodeVisitor<T>
{
    T Visit(FeatureSetMatchNode node);
    T Visit(SoundMatchNode node);
}