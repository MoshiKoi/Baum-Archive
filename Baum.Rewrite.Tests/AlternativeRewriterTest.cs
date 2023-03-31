namespace Baum.Rewrite.Tests;

public class AlternativeRewriterTest
{
    [Fact]
    void Test1()
    {
        AlternativeRewriter<char> rewriter = new() {
            new MatchRewriter<char>('a', "b"),
            new MatchRewriter<char>('b', "a")
        };

        var result1 = string.Concat(rewriter.Rewrite("a", 0).First().Rewrite);
        var result2 = string.Concat(rewriter.Rewrite("b", 0).First().Rewrite);

        Assert.Equal("b", result1);
        Assert.Equal("a", result2);
    }
}