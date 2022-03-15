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
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
        if (specifications == null) throw new ArgumentNullException(nameof(specifications));

        var openingBrackets = value.IndexesOf(specifications.Brackets.Opening);
        var closingBrackets = value.IndexesOf(specifications.Brackets.Closing);

        if (openingBrackets.Count != closingBrackets.Count) throw new Exception($"Can't extract tags : There should be the same amount of opening and closing brackets in '{value}' but there are {openingBrackets.Count} and {closingBrackets.Count} respectively.");

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