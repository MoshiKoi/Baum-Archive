using System.Text;
using System.Text.RegularExpressions;

namespace Baum.Phonology;

public class SoundChange
{
    public abstract record Action;
    public sealed record InsertAction(IEnumerable<Feature> Features) : Action;
    public sealed record DeleteAction : Action;
    public sealed record ChangeAction(IEnumerable<Feature> Included, IEnumerable<Feature> Excluded) : Action;

    public static SoundChange FromString(string rule, PhonologyData data)
        => new SoundChangeConverter(new(data), data).Convert(Notation.NotationParser.Parse(rule, data));

    public required PhonologyData PhonologyData { get; set; }
    public required Regex Regex;
    public required List<Action> Actions;

    public string Apply(string str)
        => Regex.Replace(str, match =>
        {
            var index = 0;
            var builder = new StringBuilder();

            foreach (var action in Actions)
            {
                switch (action)
                {
                    case ChangeAction change:
                        var features = PhonologyData.GetSound(match.ValueSpan[index]).Features
                            .Except(change.Excluded)
                            .Union(change.Included);

                        builder.Append(PhonologyData.GetSound(features).Symbol);
                        ++index;
                        break;
                    case InsertAction insert:
                        builder.Append(PhonologyData.GetSound(insert.Features).Symbol);
                        break;
                    case DeleteAction:
                        ++index;
                        break;
                }
            }

            return builder.ToString();
        });
}