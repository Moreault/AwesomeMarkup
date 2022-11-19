namespace ToolBX.AwesomeMarkup.Parsing;

public record MarkupTag
{
    public required string Name
    {
        get => _name;
        init
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            _name = value;
        }
    }
    private readonly string _name = null!;

    public string Value { get; init; } = string.Empty;
    public IReadOnlyList<MarkupParameter> Attributes { get; init; } = Array.Empty<MarkupParameter>();

    public required TagKind Kind { get; init; }

    public virtual bool Equals(MarkupTag? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(other.Name) || string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase)) &&
               (string.IsNullOrWhiteSpace(Value) && string.IsNullOrWhiteSpace(other.Value) || string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase)) &&
               Attributes.SequenceEqual(other.Attributes) &&
               Kind == other.Kind;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Value, Attributes, Kind);
    }

    public override string ToString()
    {
        var namePart = string.IsNullOrWhiteSpace(Value) ? Name : $"{Name}={Value}";
        return !Attributes.Any() ? namePart : $"{namePart} {string.Join(' ', Attributes)}";
    }
}