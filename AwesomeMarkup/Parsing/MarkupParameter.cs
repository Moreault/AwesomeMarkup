namespace ToolBX.AwesomeMarkup.Parsing;

public sealed record MarkupParameter
{
    public required string Name
    {
        get;
        init => field = string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException(nameof(value)) : value;
    } = null!;

    public string Value { get; init; } = string.Empty;

    public bool Equals(MarkupParameter? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
               (string.IsNullOrWhiteSpace(Value) && string.IsNullOrWhiteSpace(other.Value) || string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase));
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Name, StringComparer.OrdinalIgnoreCase);
        hash.Add(string.IsNullOrWhiteSpace(Value) ? string.Empty : Value, StringComparer.OrdinalIgnoreCase);
        return hash.ToHashCode();
    }

    public override string ToString() => string.IsNullOrWhiteSpace(Value) ? Name : $"{Name}={Value}";
}
