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
}