namespace Baum.Phonology.Notation;

public ref struct NotationParser
{

    public static SoundChange Parse(string source)
        => new NotationParser(source).Parse();

    int _index;
    ReadOnlySpan<char> _source;

    char CurrentChar => _source[_index];
    public NotationParser(string source)
    {
        _index = 0;
        _source = source.AsSpan();
    }

    public SoundChange Parse()
    {
        var match = NextMatchNode();
        Consume('>');
        var replace = NextPrimary();

        return new SoundChange
        {
            Regex = new(match.Accept(IPARegexBuilder.Instance)),
            Actions = new() { (Action.Change, replace.Included, replace.Excluded) }
        };
    }

    MatchNode NextMatchSequence()
    {
        throw new NotImplementedException();
    }

    // MatchNode : Primary
    //           | OrSet
    MatchNode NextMatchNode()
    {
        return NextPrimary();
    }

    // Primary : IPASymbol
    //         | FeatureSet
    FeatureSetMatchNode NextPrimary()
    {
        var result = FeatureSetMatchNode.ExactMatch(IPA.FromIPA(CurrentChar));
        Advance();
        return result;
    }

    void Consume(char c)
    {
        if (_index < _source.Length && CurrentChar == c)
        {
            Advance();
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    void Advance()
    {
        do
        {
            ++_index;
        }
        while (_index < _source.Length && char.IsWhiteSpace(_source[_index]));
    }
}