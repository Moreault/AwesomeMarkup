namespace ToolBX.AwesomeMarkup.Parsing;

public interface IMarkupParser
{
    IReadOnlyList<MetaString> Parse(string value, MarkupLanguageSpecifications? specifications = null);
}

[AutoInject]
public class MarkupParser : IMarkupParser
{
    private readonly IMarkupExtractor _markupExtractor;
    private readonly IMarkupTagLinker _markupTagLinker;

    public MarkupParser(IMarkupExtractor markupExtractor, IMarkupTagLinker markupTagLinker)
    {
        _markupExtractor = markupExtractor;
        _markupTagLinker = markupTagLinker;
    }

    //TODO Support "processing" tags (ex : <?version?>)
    //TODO Disallow using unescaped tag brackets inside tags
    //TODO Allow spaces (or whatever attribute separator) inside attribute values that are surrounded by quotes
    //TODO Escapable brackets (whatever the brackets used... possibly have escape characters defined in the specs class)
    //TODO Support single self-closing tags ex : <br />
    public IReadOnlyList<MetaString> Parse(string value, MarkupLanguageSpecifications? specifications = null)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
        specifications ??= MarkupLanguageSpecifications.Dml;

        var tags = _markupExtractor.Extract(value, specifications);
        var linkedTags = _markupTagLinker.Link(tags);

        var metaStrings = new List<MetaString>();
        var currentIndex = 0;
        while (currentIndex < value.Length)
        {
            if (value[currentIndex] == specifications.Brackets.Opening)
            {
                var currentLink = linkedTags.Single(x => x.StartIndex == currentIndex);
                var nestedMetaStrings = Parse(value.Substring(currentLink.Opening.EndIndex + 1, currentLink.Closing.StartIndex - currentLink.Opening.EndIndex - 1));

                currentIndex = currentLink.EndIndex + 1;

                if (nestedMetaStrings.Any())
                {
                    if (!nestedMetaStrings.First().Tags.Any())
                        metaStrings.Add(new MetaString
                        {
                            Tags = new List<MarkupTag> { currentLink.Opening.Tag },
                            Text = nestedMetaStrings.First().Text
                        });

                    if (nestedMetaStrings.Count >= 1)
                    {
                        var otherStrings = nestedMetaStrings.Select(x => new MetaString
                        {
                            Tags = new List<MarkupTag> { currentLink.Opening.Tag }.Concat(x.Tags).ToList(),
                            Text = x.Text
                        }).ToList();

                        foreach (var meta in metaStrings)
                        {
                            var indexOfMeta = otherStrings.IndexOf(meta);
                            if (indexOfMeta > -1)
                                otherStrings.RemoveAt(indexOfMeta);
                        }

                        metaStrings.AddRange(otherStrings);
                    }
                }

            }
            else
            {
                var indexOfNextBracket = value.IndexOf(specifications.Brackets.Opening, currentIndex);
                metaStrings.Add(new MetaString
                {
                    Text = indexOfNextBracket == -1 ? value.Substring(currentIndex) : value.Substring(currentIndex, indexOfNextBracket - currentIndex)
                });
                currentIndex = indexOfNextBracket == -1 ? value.Length : indexOfNextBracket;
            }

        }

        return metaStrings;
    }
}