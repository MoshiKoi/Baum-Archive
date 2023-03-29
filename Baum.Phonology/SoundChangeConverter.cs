using System.Text.RegularExpressions;
using Baum.Phonology.Notation;

namespace Baum.Phonology;

public class SoundChangeConverter
{
    IPARegexBuilder RegexBuilder { get; }
    PhonologyData Data { get; }
    public SoundChangeConverter(IPARegexBuilder regexBuilder, PhonologyData data)
        => (RegexBuilder, Data) = (regexBuilder, data);

    public SoundChange Convert(SoundChangeNode soundChangeNode)
    {
        var regex = new Regex(soundChangeNode.Match.Accept(RegexBuilder));

        return new SoundChange
        {
            PhonologyData = Data,
            Regex = regex,
            Actions = soundChangeNode.Replace switch {
                SoundMatchNode node => new()
                {
                    new SoundChange.DeleteAction(),
                    new SoundChange.InsertAction(node.Features)
                },
                _ => throw new NotImplementedException()
            }
        };
    }
}