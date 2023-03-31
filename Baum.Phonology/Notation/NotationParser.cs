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
        var replace = NextMatchNode();

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
        while (_isValid && _tokens.Current is SoundToken or OpenBracket or OpenBrace)
        {
            matchNodes.Add(NextMatchNode());
        }
        return matchNodes;
    }

    // MatchNode : IPASymbol
    //           | FeatureSet
    //           | List
    MatchNode NextMatchNode()
    {
        switch (CurrentToken)
        {
            case SoundToken { Features: var features }:
                Advance();
                return new SoundMatchNode(features);
            case OpenBracket:
                return NextFeatureSet();
            case OpenBrace:
                return NextMatchList();
            default:
                throw new NotImplementedException();
        }
    }

    FeatureSetMatchNode NextFeatureSet()
    {
        Advance();
        HashSet<Feature> included = new(), excluded = new();
        while (true)
        {
            if (CurrentToken is PositiveFeature { Feature: var positive })
            {
                Advance();
                included.Add(positive);
            }
            else if (CurrentToken is NegativeFeature { Feature: var negative })
            {
                Advance();
                excluded.Add(negative);
            }
            else
            {
                break;
            }
            if (CurrentToken is Comma)
                Advance();
        }
        Consume<CloseBracket>();
        return new FeatureSetMatchNode(included, excluded);
    }

    MatchNode NextMatchList()
    {
        Consume<OpenBrace>();
        List<MatchNode> nodes = new();
        while (true)
        {
            if (CurrentToken is SoundToken or OpenBracket or OpenBrace)
            {
                nodes.Add(NextMatchNode());
            }
            else
            {
                break;
            }
            if (CurrentToken is Comma)
                Advance();
        }
        Consume<CloseBrace>();
        if (nodes.Any())
            return new MatchListNode(nodes);
        else
            return new EmptyNode();
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