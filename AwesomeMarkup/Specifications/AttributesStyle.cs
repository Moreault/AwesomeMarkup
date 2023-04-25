namespace ToolBX.AwesomeMarkup.Specifications;

public record AttributesStyle
{
    public char Separator { get; init; } = ' ';

    public char Assignation { get; init; } = '=';

    public QuoteRules QuoteRules { get; init; } = new();
}