namespace ToolBX.AwesomeMarkup.Specifications;

public record QuoteRules
{
    /// <summary>
    /// Alllows/Disallows attributes values surrounded by single quotes. Ex : red='255'
    /// </summary>
    public bool Single { get; init; } = true;

    /// <summary>
    /// Alllows/Disallows attributes values surrounded by double quotes. Ex : red="255"
    /// </summary>
    public bool Double { get; init; } = true;

    /// <summary>
    /// Allows/Disallows attribute values with no quotes. Ex : red=255
    /// </summary>
    public bool Quoteless { get; init; } = true;

    /// <summary>
    /// Allows/Disallows attribute values with different quote styles in the same document. Ex : red='255' blue="50"
    /// </summary>
    public bool MultipleStyles { get; init; } = true;
}