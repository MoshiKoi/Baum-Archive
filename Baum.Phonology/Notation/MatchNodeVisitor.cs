namespace Baum.Phonology.Notation;

public interface IMatchNodeVisitor<T>
{
    T Visit(FeatureSetMatchNode node);
    T Visit(SoundMatchNode node);
    T Visit(MatchListNode node);
    T Visit(EmptyNode node);
    T Visit(EndMatchNode node);
}