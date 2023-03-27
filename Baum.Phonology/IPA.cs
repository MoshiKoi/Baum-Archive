namespace Baum.Phonology;

public static class IPA
{
    public static readonly Dictionary<char, Features> IPADict = new()
    {
        {'p', Features.Pulmonic},
        {'b', Features.Pulmonic | Features.Voiced},
        {'f', Features.Fricative},
        {'v', Features.Fricative | Features.Voiced},

        {'a', Features.Vowel | Features.Front | Features.Open},
        {'e', Features.Vowel | Features.Front | Features.CloseMid}
    };

    public static Features FromIPA(char symbol) => IPA.IPADict[symbol];

    public static char ToIPA(Features features)
        => Enumerable.Single(
            from keyValuePair in IPADict
            where keyValuePair.Value == features
            select keyValuePair.Key);
}