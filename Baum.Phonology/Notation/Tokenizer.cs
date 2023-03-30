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

record EmptyToken : Token
{
    private EmptyToken() { }
    public static readonly EmptyToken Default = new();
}

record SoundToken(IReadOnlySet<Feature> Features) : Token;
record FeatureSetToken(IReadOnlySet<Feature> Included, IReadOnlySet<Feature> Excluded) : Token;

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
                case '>':
                    ++_pos;
                    _current = DerivationSymbol.Default;
                    return true;
                case '_':
                    ++_pos;
                    _current = Underscore.Default;
                    return true;
                case '/':
                    ++_pos;
                    _current = Slash.Default;
                    return true;
                case '{':
                    ++_pos;
                    if (_pos < _source.Length && _source[_pos] == '}')
                    {
                        ++_pos;
                        _current = EmptyToken.Default;
                        return true;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                case '[':
                    _current = NextFeatureSetToken();
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

    FeatureSetToken NextFeatureSetToken()
    {
        ++_pos;
        HashSet<Feature> included = new(), excluded = new();
        do
        {
            while (char.IsWhiteSpace(_source, _pos))
                ++_pos;

            if (_source[_pos] == '+')
            {
                ++_pos;
                included.Add(NextFeature());
            }
            else if (_source[_pos] == '-')
            {
                ++_pos;
                excluded.Add(NextFeature());
            }
            else
            {
                throw new Exception();
            }
        } while (_source[_pos] != ']');
        ++_pos;


        return new FeatureSetToken(included, excluded);
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