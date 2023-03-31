using System.Collections;

namespace Baum.Phonology.Notation;

record Token;

record DerivationSymbol : Token
{
    private DerivationSymbol() { }
    public static readonly DerivationSymbol Default = new();
}

record Underscore : Token
{
    private Underscore() { }
    public static readonly Underscore Default = new();
}

record Slash : Token
{
    private Slash() { }
    public static readonly Slash Default = new();
}

record OpenBracket : Token
{
    private OpenBracket() { }
    public static readonly OpenBracket Default = new();
}

record CloseBracket : Token
{
    private CloseBracket() { }
    public static readonly CloseBracket Default = new();
}

record OpenBrace : Token
{
    private OpenBrace() { }
    public static readonly OpenBrace Default = new();
}

record CloseBrace : Token
{
    private CloseBrace() { }
    public static readonly CloseBrace Default = new();
}

record Comma : Token
{
    private Comma() { }
    public static readonly Comma Default = new();
}

record EndToken : Token
{
    private EndToken() { }
    public static readonly EndToken Default = new();
} 

record PositiveFeature(Feature Feature) : Token;
record NegativeFeature(Feature Feature) : Token;

record SoundToken(IReadOnlySet<Feature> Features) : Token;

class Tokenizer
{
    PhonologyData _data;
    public Tokenizer(PhonologyData data) => _data = data;
    public Tokenization Tokenize(string source) => new Tokenization(source, _data);
}

class Tokenization : IEnumerable<Token>
{
    string _source;
    PhonologyData _data;

    public Tokenization(string source, PhonologyData data)
        => (_source, _data) = (source, data);

    public IEnumerator<Token> GetEnumerator() => new SoundEnumerator(_source, _data);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

class SoundEnumerator : IEnumerator<Token>
{
    PhonologyData _data;
    string _source;
    int _pos;
    Token? _current;

    public SoundEnumerator(string source, PhonologyData data)
    {
        _data = data;
        _source = source;
        _pos = 0;
    }

    public Token Current => _current ?? throw new InvalidOperationException();

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        while (_pos < _source.Length)
        {
            switch (_source[_pos])
            {
                case char c when char.IsWhiteSpace(c): ++_pos; continue;
                case '>': ++_pos; _current = DerivationSymbol.Default; return true;
                case '_': ++_pos; _current = Underscore.Default; return true;
                case '/': ++_pos; _current = Slash.Default; return true;
                case '[': ++_pos; _current = OpenBracket.Default; return true;
                case ']': ++_pos; _current = CloseBracket.Default; return true;
                case '{': ++_pos; _current = OpenBrace.Default; return true;
                case '}': ++_pos; _current = CloseBrace.Default; return true;
                case ',': ++_pos; _current = Comma.Default; return true;
                case '#': ++_pos; _current = EndToken.Default; return true;
                case '+':
                    ++_pos;
                    _current = new PositiveFeature(NextFeature());
                    return true;
                case '-':
                    ++_pos;
                    _current = new NegativeFeature(NextFeature());
                    return true;
                default:
                    var sound = _data.GetStartSound(_source.Substring(_pos));
                    _pos += sound.Symbol.Length;
                    _current = new SoundToken(sound.Features);
                    return true;
            }
        }
        return false;
    }

    Feature NextFeature()
    {
        var start = _pos;

        while (char.IsLetter(_source, _pos))
            ++_pos;

        return new Feature(_source[start.._pos].ToString());
    }

    public void Reset() => _pos = 0;

    public void Dispose() { }
}