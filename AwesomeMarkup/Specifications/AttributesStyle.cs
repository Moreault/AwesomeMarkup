namespace ToolBX.AwesomeMarkup.Specifications;

public record AttributesStyle
{
    public char Separator { get; init; } = ' ';

    public char AssignationSeparator { get; init; } = '=';

    public QuoteRules QuoteRules { get; init; } = QuoteRules.AllowDoubleQuotes & QuoteRules.AllowSingleQuotes & QuoteRules.AllowNoQuotes;

    public StringComparison NameComparison { get; init; } = StringComparison.InvariantCultureIgnoreCase;
}