namespace ToolBX.AwesomeMarkup.Conversion;

public record LinkedTag(MarkupTagInfo Opening, MarkupTagInfo Closing)
{
    public MarkupTagInfo Opening { get; init; } = Opening ?? throw new ArgumentNullException(nameof(Opening));
    public MarkupTagInfo Closing { get; init; } = Closing ?? throw new ArgumentNullException(nameof(Closing));

    public int StartIndex => Opening.StartIndex;
    public int EndIndex => Closing.EndIndex;
}