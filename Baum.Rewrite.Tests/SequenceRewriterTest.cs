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