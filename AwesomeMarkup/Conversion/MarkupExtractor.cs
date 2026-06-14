namespace ToolBX.AwesomeMarkup.Conversion;

public interface IMarkupExtractor
{
    IReadOnlyList<MarkupTagInfo> Extract(string value, MarkupLanguageSpecifications specifications);
}

[AutoInject]
public class MarkupExtractor : IMarkupExtractor
{
    private readonly IMarkupTagConverter _markupTagConverter;

    public MarkupExtractor(IMarkupTagConverter markupTagConverter)
    {
        _markupTagConverter = markupTagConverter;
    }

    public IReadOnlyList<MarkupTagInfo> Extract(string value, MarkupLanguageSpecifications specifications)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw string.IsNullOrEmpty(value)
                ? new ArgumentNullException(nameof(value))
                : new ArgumentException(Exceptions.ValueCannotBeWhitespace, nameof(value));
        if (specifications == null) throw new ArgumentNullException(nameof(specifications));

        var openingBrackets = value.IndexesOf(specifications.Brackets.Opening);
        var closingBrackets = value.IndexesOf(specifications.Brackets.Closing);

        if (openingBrackets.Count != closingBrackets.Count) throw new MarkupParsingException(string.Format(Exceptions.MismatchedBracketCount, value, openingBrackets.Count, closingBrackets.Count));

        var tagInfo = new List<MarkupTagInfo>();
        for (var i = 0; i < openingBrackets.Count; i++)
        {
            var startIndex = openingBrackets[i];
            var endIndex = closingBrackets[i];

            tagInfo.Add(new MarkupTagInfo
            {
                StartIndex = startIndex,
                EndIndex = endIndex,
                Tag = _markupTagConverter.Convert(value.Substring(startIndex, endIndex - startIndex + 1), specifications),
            });
        }

        return tagInfo;
    }
}