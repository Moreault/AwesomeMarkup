namespace ToolBX.AwesomeMarkup.Conversion;

public interface IMarkupAttributeExtractor
{
    IReadOnlyList<string> Extract(string value, MarkupLanguageSpecifications specifications);
}

[AutoInject]
public class MarkupAttributeExtractor : IMarkupAttributeExtractor
{
    public IReadOnlyList<string> Extract(string value, MarkupLanguageSpecifications specifications)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (specifications == null) throw new ArgumentNullException(nameof(specifications));
        if (string.IsNullOrWhiteSpace(value)) return Array.Empty<string>();

        var splits = value.IndexesOf(specifications.Attributes.Separator);
        var singleQuotes = value.IndexesOf('\'');
        if (singleQuotes.Count % 2 != 0) throw new MarkupParsingException(string.Format(Exceptions.OddNumberOfQuotes, singleQuotes.Count));

        var doubleQuotes = value.IndexesOf('\"');
        if (doubleQuotes.Count % 2 != 0) throw new MarkupParsingException(string.Format(Exceptions.OddNumberOfQuotes, doubleQuotes.Count));

        var ranges = new List<Range<int>>();

        var index = 0;
        while (index < singleQuotes.Count)
        {
            var first = singleQuotes[index];
            var next = singleQuotes[index + 1];
            ranges.Add(new Range<int>(first, next));
            index += 2;
        }

        index = 0;
        while (index < doubleQuotes.Count)
        {
            var first = doubleQuotes[index];
            var next = doubleQuotes[index + 1];
            ranges.Add(new Range<int>(first, next));
            index += 2;
        }

        var actualSplits = splits.Where(split => !ranges.Any(x => x.Start <= split && x.End >= split)).Concat(value.LastIndex() + 1).ToList();

        var output = new List<string>();
        var lastSplit = 0;
        foreach (var split in actualSplits)
        {
            output.Add(value.Substring(lastSplit, split - lastSplit));
            lastSplit = split + 1;
        }
        return output;
    }
}