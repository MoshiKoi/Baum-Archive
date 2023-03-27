namespace Baum.Phonology;

[Flags]
public enum Features : ulong
{
    None = 0,
    All = ulong.MaxValue,
    Pulmonic = 1 << 1,
    Fricative = 1 << 2,
    Voiced = 1 << 3,

    Vowel = 1 << 8,
    Central = 1 << 9,
    Back = 1 << 10,
    Front = 1 << 11,
    CloseMid = 1 << 12,
    Open = 1 << 13,
}


static class FeaturesExtensions
{
    public static Features Without(this Features self, Features features) => self & ~features;
    public static Features With(this Features self, Features features) => self | features;

    // public static bool Has(this Features self, Features features) => (self & features) == features;
}