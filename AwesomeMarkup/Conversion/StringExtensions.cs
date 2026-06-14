namespace ToolBX.AwesomeMarkup.Conversion;

internal static class StringExtensions
{
    //TODO Make more "generic" and put it in SmartyStrings(?)
    internal static IReadOnlyList<string> SplitWithQuotes(this string value, char separator)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (string.IsNullOrWhiteSpace(value)) return Array.Empty<string>();

        var splits = value.IndexesOf(separator);
        var singleQuotes = value.IndexesOf('\'');
        if (singleQuotes.Count % 2 != 0) throw new MarkupParsingException(string.Format(Exceptions.OddNumberOfQuotes, singleQuotes.Count));

        var doubleQuotes = value.IndexesOf('\"');
        if (doubleQuotes.Count % 2 != 0) throw new MarkupParsingException(string.Format(Exceptions.OddNumberOfQuotes, doubleQuotes.Count));

        var quotedIndices = new HashSet<int>();

        for (var i = 0; i < singleQuotes.Count; i += 2)
        {
            for (var j = singleQuotes[i]; j <= singleQuotes[i + 1]; j++)
                quotedIndices.Add(j);
        }

        for (var i = 0; i < doubleQuotes.Count; i += 2)
        {
            for (var j = doubleQuotes[i]; j <= doubleQuotes[i + 1]; j++)
                quotedIndices.Add(j);
        }

        var actualSplits = splits.Where(split => !quotedIndices.Contains(split)).Concat(value.LastIndex() + 1).ToList();

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
