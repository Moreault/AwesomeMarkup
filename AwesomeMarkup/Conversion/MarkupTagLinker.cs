using ToolBX.AwesomeMarkup.Resources;

namespace ToolBX.AwesomeMarkup.Conversion;

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
            var openingTag = tempList.Last(x => x.Kind is TagKind.Opening or TagKind.SelfClosing or TagKind.Processing);

            var closingTag = tempList.FirstOrDefault(x => x.Kind is TagKind.Closing && x.StartIndex > openingTag.EndIndex && string.Equals(x.Tag.Name, openingTag.Tag.Name, StringComparison.InvariantCultureIgnoreCase));
            if (closingTag == null && !openingTag.IsClosing) throw new MarkupParsingException(string.Format(Exceptions.OpeningTagWithoutClosingTag, openingTag.Tag.Name));

            linked.Add(closingTag == null ? new LinkedTag(openingTag) : new LinkedTag(openingTag, closingTag));

            tempList.Remove(openingTag);

            if (closingTag != null)
                tempList.Remove(closingTag);
        }

        return linked.OrderBy(x => x.Opening.StartIndex).ToList();
    }
}