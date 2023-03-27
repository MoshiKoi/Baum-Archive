using Baum.Phonology;

namespace Baum.Phonology.Tests;

public class SoundChangeTest
{
    [Fact]
    public void VoicingAddition()
    {
        var change = new SoundChange
        {
            Regex = new("p"),
            Actions = new() { (Action.Change, Features.Voiced, Features.None) }
        };

        var result = change.Apply("pat");
        Assert.Equal("bat", result);
    }

    [Fact]
    public void Devoicing()
    {
        var change = new SoundChange
        {
            Regex = new("b"),
            Actions = new() { (Action.Change, Features.None, Features.Voiced) }
        };

        var result = change.Apply("bat");
        Assert.Equal("pat", result);
    }

    [Fact]
    public void Insertion()
    {
        var change = new SoundChange
        {
            Regex = new("(?<=p)(?=t)"),
            Actions = new() { (Action.Insert, IPA.FromIPA('a'), Features.None) }
        };

        var result = change.Apply("pt");
        Assert.Equal("pat", result);
    }

    [Fact]
    public void Deletion()
    {
        var change = new SoundChange
        {
            Regex = new("(?<=p)a(?=t)"),
            Actions = new() { (Action.Delete, Features.None, Features.None) }
        };

        var result = change.Apply("pat");
        Assert.Equal("pt", result);
    }
}