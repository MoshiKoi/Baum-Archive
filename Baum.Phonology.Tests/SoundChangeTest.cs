using Baum.Phonology;

namespace Baum.Phonology.Tests;

public class SoundChangeTest
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
    public void VoicingAddition()
    {
        var change = new SoundChange
        {
            PhonologyData = stubData,
            Regex = new("p"),
            Actions = new() {
                new SoundChange.ChangeAction(new Feature[] { new("voiced") }, Array.Empty<Feature>())
            }
        };

        var result = change.Apply("pat");
        Assert.Equal("bat", result);
    }

    [Fact]
    public void Devoicing()
    {
        var change = new SoundChange
        {
            PhonologyData = stubData,
            Regex = new("b"),
            Actions = new() {
                new SoundChange.ChangeAction(Array.Empty<Feature>(), new Feature[] { new("voiced") })
            }
        };

        var result = change.Apply("bat");
        Assert.Equal("pat", result);
    }

    [Fact]
    public void Insertion()
    {
        var change = new SoundChange
        {
            PhonologyData = stubData,
            Regex = new("(?<=p)(?=t)"),
            Actions = new() {
                new SoundChange.InsertAction(new Feature[] { new("vowel"), new("open") })
            }
        };

        var result = change.Apply("pt");
        Assert.Equal("pat", result);
    }

    [Fact]
    public void Deletion()
    {
        var change = new SoundChange
        {
            PhonologyData = stubData,
            Regex = new("(?<=p)a(?=t)"),
            Actions = new() { new SoundChange.DeleteAction() }
        };

        var result = change.Apply("pat");
        Assert.Equal("pt", result);
    }
}