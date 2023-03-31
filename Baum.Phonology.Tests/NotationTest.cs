namespace Baum.Phonology.Tests;

public class NotationTest
{
    PhonologyData stubData = new(new[]
    {
        new Sound("a", new HashSet<Feature>() { new("vowel"), new("open")}),
        new Sound("e", new HashSet<Feature>() { new("vowel"), new("close-mid") }),
        new Sound("m̥", new HashSet<Feature>() { new("consonant"), new("nasal") }),
        new Sound("m", new HashSet<Feature>() { new("consonant"), new("nasal"), new("voiced") }),
        new Sound("p", new HashSet<Feature>() { new("consonant"), new("plosive") } ),
        new Sound("b", new HashSet<Feature>() { new("consonant"), new("plosive"), new("voiced") } ),
    });

    [Theory]
    [InlineData("p>b", "pam", "bam")]
    [InlineData("p>b", "papam", "babam")]
    [InlineData("a>e", "pam", "pem")]
    public void SimpleNotationPasses(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("p > b / a_", "pape", "pabe")]
    [InlineData("p > b / a_", "pamap", "pamab")]
    [InlineData("a > e / [-voiced]_", "pap", "pep")]
    [InlineData("a > e / [-voiced]_", "map", "map")]
    public void PreconditionNotationPasses(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("p > b / _a", "pape", "bape")]
    [InlineData("p > b / _a", "pamap", "bamap")]
    [InlineData("a > e / _[-voiced]", "pap", "pep")]
    [InlineData("a > e / _[-voiced]", "pam", "pam")]
    public void PostconditionNotationPasses(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("p > {}", "pamep", "ame")]
    public void UnconditionalDeletionPasses(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("{} > p / e_", "eme", "epmep")]
    public void PreconditionInsertionPasses(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("{} > p / p_", "pep")]
    public void InfiniteSoundChangeFails(string rule, string initial)
    {
        Assert.False(SoundChange.TryApply(initial, rule, stubData, out var result));
    }

    [Theory]
    [InlineData("p > [+voiced]", "pem", "bem")]
    public void AddingFeaturesPasses(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("b > [-voiced]", "bem", "pem")]
    public void SubtractingFeaturesPasses(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("[+voiced] > m", "peb", "pem")]
    [InlineData("[+plosive -voiced] > {}", "peb", "eb")]
    public void MatchingFeaturesPasses(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("{a,e,{p,b}} > {e,a,{b,p}}", "pabe", "bepa")]
    public void MappingBracedList(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("{p,b} > m", "pabe", "mame")]
    public void BracedListToSound(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("{b,m} > [-voiced]", "bame", "pam̥e")]
    [InlineData("{p,m̥} > [+voiced]", "pam̥e", "bame")]
    public void BracedListToFeatureChange(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData("p > b / _{a,e}", "papepm", "babepm")]
    [InlineData("p > b / _{e,#}", "pep", "beb")]
    public void BracedListCondition(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("e > {} / _#", "peme", "pem")]
    public void WordEndPostCondition(string rule, string initial, string expected)
    {
        Assert.True(SoundChange.TryApply(initial, rule, stubData, out var result));
        Assert.Equal(expected, result);
    }
}