using Baum.Phonology;

namespace Baum.Phonology.Tests;

public class SoundTest
{
    [Fact]
    public void OrderOfFeaturesDoesNotMatterForEquality()
    {
        Sound firstSound = new("x", new HashSet<Feature> { new("a"), new("b"), new("c") });
        Sound secondSound = new("x", new HashSet<Feature> { new("b"), new("c"), new("a") });

        Assert.True(firstSound == secondSound);
        Assert.True(secondSound == firstSound);

        Assert.True(firstSound.Equals(secondSound));
        Assert.True(secondSound.Equals(firstSound));

        Assert.Equal(firstSound, secondSound);
    }
}