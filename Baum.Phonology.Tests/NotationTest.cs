using Baum.Phonology.Notation;

namespace Baum.Phonology.Tests;

public class NotationTest
{
    PhonologyData stubData = new(new[]
    {
        new Sound('a', new HashSet<Feature>() { new("vowel"), new("open")}),
        new Sound('e', new HashSet<Feature>() { new("vowel"), new("close-mid") }),
        new Sound('m', new HashSet<Feature>() { new("consonant"), new("nasal") } ),
        new Sound('p', new HashSet<Feature>() { new("consonant"), new("plosive") } ),
        new Sound('b', new HashSet<Feature>() { new("consonant"), new("plosive"), new("voiced") } ),
    });

    [Theory]
    [InlineData("p>b", "pat", "bat")]
    [InlineData("p>b", "papat", "babat")]
    [InlineData("a>e", "pat", "pet")]
    public void SimpleNotationPasses(string rule, string initial, string expected)
    {
        var regexBuilder = new IPARegexBuilder(stubData);
        var converter = new SoundChangeConverter(regexBuilder, stubData);
        var soundChange = converter.Convert(NotationParser.Parse(rule, stubData));

        var result = soundChange.Apply(initial);

        Assert.Equal(expected, result);
    }
}