namespace ToolBX.AwesomeMarkup.Parsing;

public record MetaString
{
    public IReadOnlyList<MarkupTag> Tags { get; init; } = Array.Empty<MarkupTag>();
    public string Text { get; init; } = string.Empty;

    public virtual bool Equals(MetaString? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Tags.SequenceEqual(other.Tags) && Text == other.Text;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Tags, Text);
    }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(Text) && Tags.Any()) return "(Empty)";
        if (string.IsNullOrWhiteSpace(Text) && !Tags.Any()) return $"Tags {string.Join(", ", Tags.Select(x => $"<{x}>"))}";
        if (Tags.Any()) return $"'{Text}' with tags {string.Join(", ", Tags.Select(x => $"<{x}>"))}";
        return $"'{Text}'";
    }
}