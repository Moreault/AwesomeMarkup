namespace ToolBX.AwesomeMarkup.Conversion;

public record MarkupTagInfo
{
    public required MarkupTag Tag { get; init; }
    public int StartIndex { get; init; }
    public int EndIndex { get; init; }
    public bool IsClosing => Tag.Name.Contains('/');

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