using Baum.Rewrite;
using Baum.Phonology.Notation;

namespace Baum.Phonology;

public class SoundChange
{
    public static bool TryApply(string initial, string rule, PhonologyData data, out string after)
    {
        try
        {
            var rewriter = NotationParser.Parse(rule, data);
            var change = new SoundChange
            {
                PhonologyData = data,
                Rewriter = rewriter
            };

            after = change.Apply(initial);
            return true;
        }
        catch (Exception) // TODO: Specialize exception
        {
            after = initial;
            return false;
        }
    }

    public required IRewriter<IReadOnlySet<Feature>> Rewriter { get; set; }
    public required PhonologyData PhonologyData { get; set; }

    string Apply(string str)
    {
        var featureString = new Tokenization(str, PhonologyData)
            .Select(sound => ((SoundToken)sound).Features);

        // TODO: Not quite sure if this algorithm is actually the best way to do this
        // Replaces every match in the string
        int maxLength = featureString.Count() * 3;
        for (int pos = 0; pos < featureString.Count(); ++pos)
        {
            // Prevents infinite insertions like {} > p / p_ where the Count keeps growing
            if (pos > maxLength)
                throw new Exception("Word tripled in length, triggering infinite loop protection");
            var rewrites = Rewriter.Rewrite(featureString, pos);
            if (rewrites.Any())
            {
                var replacement = rewrites.MaxBy(pair => pair.RewritePosition);
                featureString = featureString
                    .Take(pos)
                    .Concat(replacement.Rewrite)
                    .Concat(featureString.Skip(replacement.RewritePosition));
            }
        }

        return string.Concat(featureString.Select(f => PhonologyData.GetSound(f).Symbol));
    }
}