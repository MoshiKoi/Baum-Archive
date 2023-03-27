using Baum.Phonology.Notation;

namespace Baum.Phonology.Tests;

public class NotationTest
{
    [Theory]
    [InlineData("p>b", "pat", "bat")]
    [InlineData("p>b", "papat", "babat")]
    [InlineData("a>e", "pat", "pet")]
    public void SimpleNotationPasses(string rule, string initial, string expected)
    {
        var soundChange = NotationParser.Parse(rule);

        var result = soundChange.Apply(initial);

        Assert.Equal(expected, result);
    }
}