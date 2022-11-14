﻿using ToolBX.AwesomeMarkup.Resources;

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
            var openingTag = tempList.Last(x => !x.IsClosing);

            var closingTag = tempList.FirstOrDefault(x => x.IsClosing && x.StartIndex > openingTag.EndIndex && string.Equals(x.Tag.Name, $"/{openingTag.Tag.Name}", StringComparison.InvariantCultureIgnoreCase));
            if (closingTag == null) throw new ArgumentException(string.Format(Exceptions.OpeningTagWithoutClosingTag, openingTag.Tag.Name));

            linked.Add(new LinkedTag(openingTag, closingTag));

            tempList.Remove(openingTag);
            tempList.Remove(closingTag);
        }

        return linked.OrderBy(x => x.Opening.StartIndex).ToList();
    }
}