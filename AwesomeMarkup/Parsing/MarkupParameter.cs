namespace ToolBX.AwesomeMarkup.Parsing;

public record MarkupParameter
{
    public required string Name
    {
        get => _name;
        init => _name = string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException(nameof(value)) : value;
    }
    private readonly string _name = null!;

    public string Value { get; init; } = string.Empty;

    public virtual bool Equals(MarkupParameter? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(other.Name) || string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase)) &&
               (string.IsNullOrWhiteSpace(Value) && string.IsNullOrWhiteSpace(other.Value) || string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Value);
    }

    public override string ToString() => string.IsNullOrWhiteSpace(Value) ? Name : $"{Name}={Value}";
}