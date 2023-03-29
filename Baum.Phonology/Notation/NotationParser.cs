namespace Baum.Phonology.Notation;

public ref struct NotationParser
{
    public static SoundChangeNode Parse(string source, PhonologyData data)
        => new NotationParser(source, data).Parse();

    IEnumerator<Token> _tokens;
    bool _isValid;
    PhonologyData _data;

    Token? CurrentToken => _isValid ? _tokens.Current : null;

    public NotationParser(string source, PhonologyData data)
    {
        _data = data;
        _tokens = new Tokenization(source, data).GetEnumerator();
        Advance();
    }

    public SoundChangeNode Parse()
    {
        var match = NextMatchNode();
        Consume<DerivationSymbol>();
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
        switch (CurrentToken)
        {
            case SoundToken token:
                var result = new SoundMatchNode(token.Features);
                Advance();
                return result;
            default:
                throw new NotImplementedException();
        }
    }

    void Consume<T>() where T : Token
    {
        if (_tokens.Current is T)
        {
            Advance();
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    void Advance() => _isValid = _tokens.MoveNext();
}