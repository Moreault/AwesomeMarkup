namespace ToolBX.AwesomeMarkup.Conversion;

public record MarkupTagInfo
{
    public required MarkupTag Tag { get; init; }
    public int StartIndex { get; init; }
    public int EndIndex { get; init; }

    public string Value => Tag.Value;
    public IReadOnlyList<MarkupParameter> Attributes => Tag.Attributes;
    public TagKind Kind => Tag.Kind;

    public bool IsClosing => Kind is TagKind.Closing or TagKind.SelfClosing or TagKind.Processing;

    public virtual bool Equals(MarkupTagInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Tag, other.Tag) &&
               StartIndex == other.StartIndex &&
               EndIndex == other.EndIndex;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Tag, StartIndex, EndIndex);
    }
}