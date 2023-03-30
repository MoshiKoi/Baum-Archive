using Baum.Rewrite;

namespace Baum.Phonology.Notation;

public ref struct NotationParser
{
    public static IRewriter<IReadOnlySet<Feature>> Parse(string source, PhonologyData data)
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

    public IRewriter<IReadOnlySet<Feature>> Parse()
    {
        var match = NextMatchNode();
        Consume<DerivationSymbol>();
        var replace = NextPrimary();

        var rewriter = replace.Accept(match.Accept(new SoundChangeRewriteParser()));

        if (_isValid)
        {
            Consume<Slash>();
            return ParseCondition(rewriter);
        }
        else
        {
            return rewriter;
        }
    }

    List<MatchNode> NextMatchSequence()
    {
        List<MatchNode> matchNodes = new();
        while (_isValid && _tokens.Current is SoundToken or FeatureSetToken or EmptyToken)
        {
            matchNodes.Add(NextMatchNode());
        }
        return matchNodes;
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
            case SoundToken { Features:var features}:
                Advance();
                return new SoundMatchNode(features);
            case FeatureSetToken { Included: var included, Excluded: var excluded }:
                Advance();
                return new FeatureSetMatchNode(included, excluded);
            case EmptyToken:
                Advance();
                return new EmptyNode();
            default:
                throw new NotImplementedException();
        }
    }

    IRewriter<IReadOnlySet<Feature>> ParseCondition(IRewriter<IReadOnlySet<Feature>> changeRewriter)
    {
        var rewriter = new SequenceRewriter<IReadOnlySet<Feature>>();

        foreach (var match in NextMatchSequence())
            rewriter.Add(match.Accept(new SoundMatchRewriteMatchParser()));

        Consume<Underscore>();
        rewriter.Add(changeRewriter);

        foreach (var match in NextMatchSequence())
            rewriter.Add(match.Accept(new SoundMatchRewriteMatchParser()));

        return rewriter;
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