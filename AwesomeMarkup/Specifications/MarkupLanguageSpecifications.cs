namespace ToolBX.AwesomeMarkup.Specifications;

public record MarkupLanguageSpecifications
{
    public static readonly MarkupLanguageSpecifications Dml = new()
    {
        Brackets = BracketStyle.Angle,
        Attributes = new AttributesStyle
        {
            Separator = ' ',
            Assignation = '=',
            QuoteRules = new QuoteRules
            {
                Single = true,
                Double = true,
                Quoteless = true,
                MultipleStyles = false
            }
        }
    };

    public static readonly MarkupLanguageSpecifications Xml = new()
    {
        Brackets = BracketStyle.Angle,
        Attributes = new AttributesStyle
        {
            Separator = ' ',
            Assignation = '=',
            QuoteRules = new QuoteRules
            {
                Single = false,
                Double = true,
                Quoteless = false,
                MultipleStyles = false
            }
        }
    };

    public BracketStyle Brackets { get; init; } = BracketStyle.Angle;

    public AttributesStyle Attributes { get; init; } = new();

    /// <summary>
    /// When set, this character escapes the next character so that brackets (and the escape character itself) can appear
    /// literally inside text and attribute values. It only escapes the opening bracket, the closing bracket and itself;
    /// in front of any other character it is treated as a literal character. Escaping is disabled when this is <c>null</c>.
    /// </summary>
    public char? EscapeCharacter { get; init; }
}