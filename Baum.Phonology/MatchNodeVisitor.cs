namespace Baum.Phonology;

public interface MatchNodeVisitor<T>
{
    T Visit(FeatureSetMatchNode node);
}