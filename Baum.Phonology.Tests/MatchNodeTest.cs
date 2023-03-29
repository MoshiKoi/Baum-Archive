using Baum.Phonology.Notation;

namespace Baum.Phonology.Tests;

public class FeatureSetTest
{
    PhonologyData stubData = new(new[]
    {
        new Sound('a', new HashSet<Feature>() { new("vowel"), new("open")}),
        new Sound('e', new HashSet<Feature>() { new("vowel"), new("close-mid") }),
        new Sound('m', new HashSet<Feature>() { new("consonant"), new("nasal") } ),
        new Sound('p', new HashSet<Feature>() { new("consonant"), new("plosive") } ),
        new Sound('b', new HashSet<Feature>() { new("consonant"), new("plosive"), new("voiced") } ),
    });

    [Fact]
    void ExactMatch()
    {
        var test = new SoundMatchNode(new HashSet<Feature>() { new("consonant"), new("plosive") });

        var regexBuilder = new IPARegexBuilder(stubData);

        var regexStr = test.Accept(regexBuilder);

        Assert.Equal("p", regexStr);
    }

    [Fact]
    void IncludedFeatures()
    {
        var test = new FeatureSetMatchNode(
            new HashSet<Feature>() { new("consonant"), new("plosive") },
            new HashSet<Feature>());

        var regexBuilder = new IPARegexBuilder(stubData);

        var regexStr = test.Accept(regexBuilder);

        Assert.Equal("[pb]", regexStr);
    }

    [Fact]
    void ExcludedFeatures()
    {
        var test = new FeatureSetMatchNode(
            new HashSet<Feature>() { },
            new HashSet<Feature>() { new("vowel"), new("plosive") });

        var regexBuilder = new IPARegexBuilder(stubData);

        var regexStr = test.Accept(regexBuilder);

        Assert.Equal("[m]", regexStr);
    }


    // p > [+voice]
    // pat > bat
    // p > [+voice] / _a
    // pat > bat, put > put


    // Or conditions
    // Syllable/word boundary conditions

}