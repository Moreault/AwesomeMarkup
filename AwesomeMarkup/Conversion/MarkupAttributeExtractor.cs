namespace ToolBX.AwesomeMarkup.Conversion;

public interface IMarkupAttributeExtractor
{
    IReadOnlyList<string> Extract(string value, MarkupLanguageSpecifications specifications);
}

public class MarkupAttributeExtractor : IMarkupAttributeExtractor
{
    public IReadOnlyList<string> Extract(string value, MarkupLanguageSpecifications specifications)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (specifications == null) throw new ArgumentNullException(nameof(specifications));
        if (string.IsNullOrWhiteSpace(value)) return Array.Empty<string>();

        return value.SplitWithQuotes(specifications.Attributes.Separator);
    }
}
