using System.Collections;

namespace Baum.Phonology.Notation;

record Token;

record DerivationSymbol : Token
{
    private DerivationSymbol() {}
    public static readonly DerivationSymbol Default = new();
};

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
                default:
                    var sound = _data.GetStartSound(_source.Substring(_pos));
                    _pos += sound.Symbol.Length;
                    _current = new SoundToken(sound.Features);
                    return true;
            }
        }
        return false;
    }

    public void Reset() => _pos = 0;

    public void Dispose() { }
}