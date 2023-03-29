namespace Baum.Phonology.Notation;

public ref struct NotationParser
{

    public static SoundChangeNode Parse(string source, PhonologyData data)
        => new NotationParser(source, data).Parse();

    int _index;
    ReadOnlySpan<char> _source;
    PhonologyData _data;

    char CurrentChar => _source[_index];
    public NotationParser(string source, PhonologyData data)
    {
        _index = 0;
        _source = source.AsSpan();
        _data = data;
    }

    public SoundChangeNode Parse()
    {
        var match = NextMatchNode();
        Consume('>');
        var replace = NextPrimary();

        return new SoundChangeNode(match, replace);
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
    MatchNode NextPrimary()
    {
        var result = new SoundMatchNode(_data.GetSound(CurrentChar).Features);
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