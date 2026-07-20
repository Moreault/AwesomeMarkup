namespace ToolBX.AwesomeMarkup.Parsing;

public sealed record MetaString
{
    public IReadOnlyList<MarkupTag> Tags { get; init; } = [];
    public string Text { get; init; } = string.Empty;

    public bool Equals(MetaString? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Tags.SequenceEqual(other.Tags) && Text == other.Text;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var tag in Tags)
            hash.Add(tag);
        hash.Add(Text);
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(Text) && Tags.Count == 0) return "(Empty)";
        if (string.IsNullOrWhiteSpace(Text) && Tags.Count > 0) return $"Tags {string.Join(", ", Tags.Select(x => $"<{x}>"))}";
        if (Tags.Count > 0) return $"'{Text}' with tags {string.Join(", ", Tags.Select(x => $"<{x}>"))}";
        return $"'{Text}'";
    }
}
