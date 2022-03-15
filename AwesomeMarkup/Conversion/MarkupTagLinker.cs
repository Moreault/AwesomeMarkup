namespace ToolBX.AwesomeMarkup.Conversion;

public record LinkedTag
{
    public MarkupTagInfo Opening { get; init; }
    public MarkupTagInfo Closing { get; init; }

    public int StartIndex => Opening.StartIndex;
    public int EndIndex => Closing.EndIndex;
}

public interface IMarkupTagLinker
{
    IReadOnlyList<LinkedTag> Link(IReadOnlyList<MarkupTagInfo> markupTagInfo);
}

[AutoInject]
public class MarkupTagLinker : IMarkupTagLinker
{
    public IReadOnlyList<LinkedTag> Link(IReadOnlyList<MarkupTagInfo> markupTagInfo)
    {
        if (markupTagInfo == null) throw new ArgumentNullException(nameof(markupTagInfo));

        var linked = new List<LinkedTag>();

        var tempList = markupTagInfo.ToList();

        while (tempList.Any())
        {
            var openingTag = tempList.Last(x => !x.IsClosing);

            var closingTag = tempList.First(x => x.IsClosing && x.StartIndex > openingTag.EndIndex && 
                                                 string.Equals(x.Tag.Name, $"/{openingTag.Tag.Name}", StringComparison.InvariantCultureIgnoreCase));

            linked.Add(new LinkedTag
            {
                Opening = openingTag,
                Closing = closingTag
            });

            tempList.Remove(openingTag);
            tempList.Remove(closingTag);
        }

        return linked.OrderBy(x => x.Opening.StartIndex).ToList();
    }
}