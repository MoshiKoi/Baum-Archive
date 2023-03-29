namespace Baum.Phonology;

public static class IPA
{
    public static readonly PhonologyData Data = new(new Sound[]
    {
        new('p', new HashSet<Feature>() { new("pulmonic") }),
        new('b', new HashSet<Feature>() { new("pulmonic"), new("voiced") }),
        new('f', new HashSet<Feature>() { new("fricative") }),
        new('v', new HashSet<Feature>() { new("fricative"), new("voiced") }),

        new('a', new HashSet<Feature>() { new("vowel"),  new("front"), new("open") }),
        new('e', new HashSet<Feature>() { new("vowel"),  new("front"), new("close-mid") })
    });
}