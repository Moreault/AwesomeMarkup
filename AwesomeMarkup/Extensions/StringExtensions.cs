namespace ToolBX.AwesomeMarkup.Extensions;

//TODO Move to some other DLL?
internal static class StringExtensions
{
    internal static IReadOnlyList<int> IndexesOf(this string instance, char value, StringComparison comparison = StringComparison.InvariantCulture)
    {
        return instance.IndexesOf(value.ToString(), comparison);
    }

    internal static IReadOnlyList<int> IndexesOf(this string instance, string value, StringComparison comparison = StringComparison.InvariantCulture)
    {
        if (string.IsNullOrWhiteSpace(instance)) throw new ArgumentNullException(nameof(instance));
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

        var minIndex = instance.IndexOf(value, comparison);
        if (minIndex < 0) return Array.Empty<int>();

        var indexes = new List<int> { minIndex };
        while (minIndex > -1)
        {
            minIndex = instance.IndexOf(value, minIndex + value.Length, comparison);
            if (minIndex > -1)
                indexes.Add(minIndex);
        }

        return indexes;
    }
}