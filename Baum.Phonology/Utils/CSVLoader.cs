namespace Baum.Phonology.Utils;

public static class CsvLoader
{
    public static async Task<IEnumerable<Sound>> LoadAsync(TextReader stream)
    {
        // TODO: Use header row for feature categories
        var headerLine = await stream.ReadLineAsync();

        if (headerLine == null)
            throw new InvalidDataException("No header row found");

        var header = headerLine.Split(',').Select(s => s.Trim());
        List<Sound> sounds = new();
        while (stream.ReadLine() is string line)
        {
            var fields = line.Split(',').Select(s => s.Trim());
            var sound = new Sound(
                fields.First(),
                new HashSet<Feature>(fields.Skip(1).Select(field => new Feature(field))));

            sounds.Add(sound);
        }

        return sounds;
    }
}