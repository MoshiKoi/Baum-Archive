using System.Text;
using System.Text.RegularExpressions;

namespace Baum.Phonology;

public enum Action
{
    Insert,
    Delete,
    Change,
}

public class SoundChange
{
    public required Regex Regex;
    public required List<(Action action, Features additions, Features subtractions)> Actions;

    public string Apply(string str)
        => Regex.Replace(str, match =>
        {
            var index = 0;
            var builder = new StringBuilder();

            foreach (var action in Actions)
            {
                switch (action.action)
                {
                    case Action.Change:
                        builder.Append(IPA.ToIPA(IPA.FromIPA(match.ValueSpan[index])
                            .Without(action.subtractions)
                            .With(action.additions)));
                        ++index;
                        break;
                    case Action.Insert:
                        builder.Append(IPA.ToIPA(action.additions));
                        break;
                    case Action.Delete:
                        ++index;
                        break;
                }
            }

            return builder.ToString();
        });
}