using Baum.Phonology.Utils;

namespace Baum.Phonology.Tests;

public class CsvLoaderTest
{
    [Fact]
    async Task SimpleCsvPasses()
    {
        var inputText =
            "Symbol,Feature1,Feature2\n" +
            "x,a,b\n" +
            "y,c,d";

        var expected = new Sound[]
        {
            new("x", new HashSet<Feature> { new("a"), new("b") }),
            new("y", new HashSet<Feature> { new("c"), new("d") })
        };

        using var reader = new StringReader(inputText);

        var sounds = await CsvLoader.LoadAsync(reader);

        Assert.Equal(expected, sounds);
    }

    [Fact]
    async Task EmptyCsvThrows()
    {
        var inputText = "";
        using var reader = new StringReader(inputText);

        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await CsvLoader.LoadAsync(reader);
        });
    }
}