using Baum.Phonology.Notation;

namespace Baum.Phonology.Tests;

public class NotationTest
{
    PhonologyData stubData = new(new[]
    {
        new Sound("a", new HashSet<Feature>() { new("vowel"), new("open")}),
        new Sound("e", new HashSet<Feature>() { new("vowel"), new("close-mid") }),
        new Sound("m", new HashSet<Feature>() { new("consonant"), new("nasal") } ),
        new Sound("p", new HashSet<Feature>() { new("consonant"), new("plosive") } ),
        new Sound("b", new HashSet<Feature>() { new("consonant"), new("plosive"), new("voiced") } ),
    });

    [Theory]
    [InlineData("p>b", "pam", "bam")]
    [InlineData("p>b", "papam", "babam")]
    [InlineData("a>e", "pam", "pem")]
    public void SimpleNotationPasses(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }
}