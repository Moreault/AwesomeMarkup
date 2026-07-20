using ToolBX.AwesomeMarkup.Parsing;

namespace AwesomeMarkup.Sample;

public interface ITestRunner
{
    void Run();
}

[AutoInject]
public sealed class TestRunner : ITestRunner
{
    private readonly ITerminal _terminal;
    private readonly IMarkupParser _markupParser;

    public TestRunner(ITerminal terminal, IMarkupParser markupParser)
    {
        _terminal = terminal;
        _markupParser = markupParser;
    }

    public void Run()
    {
        Demonstrate("Simple color tag", "The base is located near <color=#252321>Behabad</color>.");
        Demonstrate("Attributes", "Some <color red=200 green=12 blue=54>text</color>.");
        Demonstrate("Nested tags", "Some <bold><underline>text</underline></bold> here.");
        Demonstrate("Self-closing tag", "Line one<br/>Line two");
        Demonstrate("Processing tag and elements", "<?xml version=\"1.0\"?><note><to>Tove</to><from>Jani</from></note>");
        Demonstrate("Quoted attribute value with spaces", "<note type=\"some thing or another\">Body</note>");
    }

    private void Demonstrate(string title, string markup)
    {
        _terminal.Write($"=== {title} ===");
        _terminal.Write(markup);
        foreach (var meta in _markupParser.Parse(markup))
            _terminal.Write($"  {meta}");
    }
}
