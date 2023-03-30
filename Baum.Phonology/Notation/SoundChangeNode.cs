namespace Baum.Phonology.Notation;

public record SoundChangeNode(
    MatchNode Match,
    MatchNode Replace,
    MatchNode? Precondition,
    MatchNode? PostCondition);