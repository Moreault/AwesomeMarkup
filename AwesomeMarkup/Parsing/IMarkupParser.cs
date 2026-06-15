namespace ToolBX.AwesomeMarkup.Parsing;

public interface IMarkupParser
{
    IReadOnlyList<MetaString> Parse(string value, MarkupLanguageSpecifications? specifications = null);
}
