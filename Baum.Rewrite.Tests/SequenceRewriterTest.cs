namespace Baum.Rewrite.Tests;

public class SequenceRewriterTest
{
    [Fact]
    public void Test1()
    {
        SequenceRewriter<char> rewriter = new() {
            new MatchRewriter<char>('a'),
            new MatchRewriter<char>('b', 'B'),
            new MatchRewriter<char>('c')
        };

        var result = string.Concat(rewriter.Rewrite("abc", 0).First().Rewrite);

        Assert.Equal("aBc", result);
    }
}