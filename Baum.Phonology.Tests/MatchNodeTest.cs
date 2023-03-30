namespace Baum.Phonology.Tests;

public class FeatureSetTest
{
    PhonologyData stubData = new(new[]
    {
        new Sound("a", new HashSet<Feature>() { new("vowel"), new("open")}),
        new Sound("e", new HashSet<Feature>() { new("vowel"), new("close-mid") }),
        new Sound("m", new HashSet<Feature>() { new("consonant"), new("nasal") } ),
        new Sound("p", new HashSet<Feature>() { new("consonant"), new("plosive") } ),
        new Sound("b", new HashSet<Feature>() { new("consonant"), new("plosive"), new("voiced") } ),
    });

    // p > [+voice]
    // pat > bat
    // p > [+voice] / _a
    // pat > bat, put > put


    // Or conditions
    // Syllable/word boundary conditions

}