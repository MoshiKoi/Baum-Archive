using Baum.Phonology;

namespace Baum.Phonology.Tests;

public class FeatureSetTest
{
    [Fact]
    void ExactMatch()
    {
        var test = FeatureSetMatchNode.ExactMatch(IPA.FromIPA('p'));

        var regexBuilder = new IPARegexBuilder();

        var regexStr = test.Accept(regexBuilder);

        Assert.Equal("[p]", regexStr);
    }


    // p > [+voice]
    // pat > bat
    // p > [+voice] / _a
    // pat > bat, put > put


    // Or conditions
    // Syllable/word boundary conditions

}