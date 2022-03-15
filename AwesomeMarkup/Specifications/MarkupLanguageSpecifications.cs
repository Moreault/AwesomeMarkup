namespace ToolBX.AwesomeMarkup.Specifications;

public record MarkupLanguageSpecifications
{
    public static readonly MarkupLanguageSpecifications Dml = new();

    public BracketStyle Brackets { get; init; } = BracketStyle.Angle;

    public AttributesStyle Attributes { get; init; } = new();
}