namespace Baum.Rewrite.Tests;

public class SequenceRewriterTest
{
    [Fact]
    public void Test1()
    {
        SequenceRewriter<char> rewriter = new() {
            new MatchRewriter<char>('a'),
            new MatchRewriter<char>('b', "XY"),
            new MatchRewriter<char>('c')
        };

        var result = string.Concat(rewriter.Rewrite("abc", 0).First().Rewrite);

        Assert.Equal("aXYc", result);
    }

    [Fact]
    public void InsertionAtEndWorks()
    {
        SequenceRewriter<char> rewriter = new() {
            new MatchRewriter<char>('a'),
            new MatchRewriter<char>('b'),
            new EndRewriter<char>("c")
        };

        var result = string.Concat(rewriter.Rewrite("ab", 0).First().Rewrite);

        Assert.Equal("abc", result);
    }

    [Fact]
    public void EndDoesntMatchNotAtEnd()
    {
        SequenceRewriter<char> rewriter = new() {
            new MatchRewriter<char>('a'),
            new EndRewriter<char>("")
        };
        Assert.Empty(rewriter.Rewrite("ab", 0));
    }

    [Fact]
    public void InsertionInBetweenMatchesWorks()
    {
        SequenceRewriter<char> rewriter = new() {
            new MatchRewriter<char>('a'),
            new EmptyRewriter<char>("b"),
            new MatchRewriter<char>('c')
        };

        var result = string.Concat(rewriter.Rewrite("ac", 0).First().Rewrite);

        Assert.Equal("abc", result);
    }
}