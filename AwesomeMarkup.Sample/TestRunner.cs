﻿using ToolBX.AwesomeMarkup.Conversion;
using ToolBX.Collections.ReadOnly;

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
        WhenStackTrace_Throw();
        XmlWithoutProcessingTags();
        NestedTags();
        SimpleColorTag();
        MultipleColorTags();
        NestedAndConsecutiveTagsOfSameName();
        ProcessingTags();
        LineBreaksAndSpaces();
        SpacesInsideAttributesSurroundedByQuotes();
        UnescapedTagBracketsInsideTags();
        UnescapedTagBracketsInsideTags();
        EscapableBrackets();
        SelfClosingTags();
    }

    private void WhenStackTrace_Throw()
    {
        //Arrange
        var value = "   at ToolBX.AwesomeMarkup.Conversion.MarkupParameterConverter.Convert(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.AwesomeMarkup.Conversion.MarkupTagConverter.Convert(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.AwesomeMarkup.Conversion.MarkupExtractor.Extract(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.AwesomeMarkup.Parsing.MarkupParser.Parse(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.DML.NET.DmlSerializer.Deserialize(String text)\r\n   at ToolBX.MisterTerminal.DmlAnsiConverter.Convert(String text)\r\n   at ToolBX.MisterTerminal.TerminalWriter.WriteWithoutBreakingLine(String text, Object[] args)\r\n   at ToolBX.MisterTerminal.TerminalWriter.Write(String text, Object[] args)\r\n   at RoughConverter.Startup.Run(IServiceProvider serviceProvider) in C:\\Users\\seran\\source\\repos\\RoughConverter\\RoughConverter\\Startup.cs:line 46\r\n   at ToolBX.AssemblyInitializer.Console.ConsoleHost.UseStartup[T]()\r\n   at Program.<Main>$(String[] args) in C:\\Users\\seran\\source\\repos\\RoughConverter\\RoughConverter\\Program.cs:line 1";

        //Act
        var action = () => _markupParser.Parse(value);

        //Assert
        action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.OpeningTagWithoutClosingTag, "Main")}");
        _terminal.Write($"{nameof(WhenStackTrace_Throw)} : Passed!");
    }

    private void XmlWithoutProcessingTags()
    {
        //Arrange
        var value = "test string <markup schema xmlns:xs=\"http://www.wjs.cum/markupSchema/\" version=\"1.0\"><text is-text=\"true\"><div><span r=\"255\" g=\"0\" b=\"0\">je suis rouge</span></div></text></markup> je ne suis plus rouge";

        //Act
        var result = _markupParser.Parse(value);

        //Assert
        result.Should().BeEquivalentTo(new List<MetaString>
            {
                new()
                {
                    Text = "test string "
                },
                new()
                {
                    Text = "je suis rouge",
                    Tags = new List<MarkupTag>
                    {
                        new()
                        {
                            Name = "markup",
                            Attributes = new List<MarkupParameter>
                            {
                                new()
                                {
                                    Name = "schema"
                                },
                                new()
                                {
                                    Name = "xmlns:xs",
                                    Value = "http://www.wjs.cum/markupSchema/"
                                },
                                new()
                                {
                                    Name = "version",
                                    Value = "1.0"
                                }
                            },
                            Kind = TagKind.Opening
                        },
                        new()
                        {
                            Name = "text",
                            Attributes = new List<MarkupParameter>
                            {
                                new()
                                {
                                    Name = "is-text",
                                    Value = "true"
                                }
                            },
                            Kind = TagKind.Opening

                        },
                        new()
                        {
                            Name = "div",
                            Kind = TagKind.Opening
                        },
                        new()
                        {
                            Name = "span",
                            Attributes = new List<MarkupParameter>
                            {
                                new()
                                {
                                    Name = "r",
                                    Value = "255"
                                },
                                new()
                                {
                                    Name = "g",
                                    Value = "0"
                                },
                                new()
                                {
                                    Name = "b",
                                    Value = "0"
                                },
                            },
                            Kind = TagKind.Opening
                        },
                    }
                },
                new()
                {
                    Text = " je ne suis plus rouge"
                }});
        _terminal.Write($"{nameof(XmlWithoutProcessingTags)} : Passed!");
    }

    private void NestedTags()
    {
        //Arrange
        var value = "The base is located near <bold><color=#252321>Behabad</color></bold>.";

        //Act
        var result = _markupParser.Parse(value);

        //Assert
        result.Should().BeEquivalentTo(new List<MetaString>
        {
            new()
            {
                Text = "The base is located near "
            },
            new()
            {
                Text = "Behabad",
                Tags = new List<MarkupTag>
                {
                    new()
                    {
                        Name = "bold",
                        Kind = TagKind.Opening
                    },
                    new()
                    {
                        Name = "color",
                        Value = "#252321",
                        Kind = TagKind.Opening
                    }
                }
            },
            new()
            {
                Text = "."
            }
        });
        _terminal.Write($"{nameof(NestedTags)} : Passed!");
    }

    private void SimpleColorTag()
    {
        //Arrange
        var value = "The base is located near <color=#252321>Behabad</color>.";

        //Act
        var result = _markupParser.Parse(value);

        //Assert
        result.Should().BeEquivalentTo(new List<MetaString>
        {
            new()
            {
                Text = "The base is located near "
            },
            new()
            {
                Text = "Behabad",
                Tags = new List<MarkupTag>
                {
                    new()
                    {
                        Name = "color",
                        Value = "#252321",
                        Kind = TagKind.Opening
                    }
                }
            },
            new()
            {
                Text = "."
            }
        });
        _terminal.Write($"{nameof(SimpleColorTag)} : Passed!");
    }

    private void MultipleColorTags()
    {
        //Arrange
        var value = "That \"base\" is on the <color red=164 green=139 blue=55>outskirts</color> of <color red=56 green=116 blue=168>Behabad</color>.";

        //Act
        var result = _markupParser.Parse(value);

        //Assert
        result.Should().BeEquivalentTo(new List<MetaString>
            {
                new()
                {
                    Text = "That \"base\" is on the "
                },
                new()
                {
                    Text = "outskirts",
                    Tags = new List<MarkupTag>
                    {
                        new()
                        {
                            Name = "color",
                            Attributes = new List<MarkupParameter>
                            {
                                new() { Name = "red", Value = "164" },
                                new() { Name = "green", Value = "139" },
                                new() { Name = "blue", Value = "55" }
                            },
                            Kind = TagKind.Opening
                        }
                    }
                },
                new()
                {
                    Text = " of "
                },
                new()
                {
                    Text = "Behabad",
                    Tags = new List<MarkupTag>
                    {
                        new()
                        {
                            Name = "color",
                            Attributes = new List<MarkupParameter>
                            {
                                new() { Name = "red", Value = "56" },
                                new() { Name = "green", Value = "116" },
                                new() { Name = "blue", Value = "168" }
                            },
                            Kind = TagKind.Opening
                        }
                    }
                },
                new()
                {
                    Text = "."
                }
            });
        _terminal.Write($"{nameof(MultipleColorTags)} : Passed!");
    }

    private void NestedAndConsecutiveTagsOfSameName()
    {
        //Arrange
        var value = "<a=1><a=2>Double nested</a></a><a=3>Unnested</a>";

        //Act
        var result = _markupParser.Parse(value);

        //Assert
        result.Should().BeEquivalentTo(new List<MetaString>
        {
            new()
            {
                Text = "Double nested",
                Tags = new List<MarkupTag>
                {
                    new()
                    {
                        Name = "a",
                        Value = "1",
                        Kind = TagKind.Opening
                    },
                    new()
                    {
                        Name = "a",
                        Value = "2",
                        Kind = TagKind.Opening
                    }
                }
            },
            new()
            {
                Text = "Unnested",
                Tags = new List<MarkupTag>
                {
                    new()
                    {
                        Name = "a",
                        Value = "3",
                        Kind = TagKind.Opening
                    }
                }
            }
        });
        _terminal.Write($"{nameof(MultipleColorTags)} : Passed!");
    }

    private void ProcessingTags()
    {
        //Arrange
        var value = """<?xml version="1.0" encoding="UTF-8"?><note><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>""";

        //Act
        var result = _markupParser.Parse(value);

        //Assert
        result.Should().BeEquivalentTo(new List<MetaString>
        {
            new()
            {
                Tags = new List<MarkupTag>
                {
                    new()
                    {
                        Name = "xml", Kind = TagKind.Processing, Attributes = new List<MarkupParameter>
                        {
                            new() { Name="version", Value = "1.0" },
                            new() { Name="encoding", Value = "UTF-8" }
                        }
                    }
                }
            },
            new()
            {
                Text = "Tove",
                Tags = new List<MarkupTag>
                {
                    new() { Name = "note", Kind = TagKind.Opening },
                    new() { Name = "to", Kind = TagKind.Opening },
                }
            },
            new()
            {
                Text = "Jani",
                Tags = new List<MarkupTag>
                {
                    new() { Name = "note", Kind = TagKind.Opening },
                    new() { Name = "from", Kind = TagKind.Opening },
                }
            },
            new()
            {
                Text = "Reminder",
                Tags = new List<MarkupTag>
                {
                    new() { Name = "note", Kind = TagKind.Opening },
                    new() { Name = "heading", Kind = TagKind.Opening },
                }
            },
            new()
            {
                Text = "Don't forget me this weekend!",
                Tags = new List<MarkupTag>
                {
                    new() { Name = "note", Kind = TagKind.Opening },
                    new() { Name = "body", Kind = TagKind.Opening },
                }
            },
        });
        _terminal.Write($"{nameof(ProcessingTags)} : Passed!");
    }

    private void LineBreaksAndSpaces()
    {
        //Arrange
        var value = """
                        <note>
                            <to>Tove</to>
                            <from>Jani</from>
                        </note>
                        """;

        //Act
        var result = _markupParser.Parse(value);

        //Assert
        result.Should().BeEquivalentTo(new List<MetaString>
            {
                new()
                {
                    Text = "Tove",
                    Tags = new List<MarkupTag>
                    {
                        new() { Name = "note", Kind = TagKind.Opening },
                        new() { Name = "to", Kind = TagKind.Opening },
                    }
                },
                new()
                {
                    Text = "Jani",
                    Tags = new List<MarkupTag>
                    {
                        new() { Name = "note", Kind = TagKind.Opening },
                        new() { Name = "from", Kind = TagKind.Opening },
                    }
                }
            });
        _terminal.Write($"{nameof(LineBreaksAndSpaces)} : Passed!");
    }

    //TODO Priority : This is what is required for AwesomeMarkup to finally step out of beta
    //TODO Allow spaces (or whatever attribute separator) inside attribute values that are surrounded by quotes
    private void SpacesInsideAttributesSurroundedByQuotes()
    {
        //Arrange
        var value = """
                        <note type="some thing or another">
                            <to>Tove</to>
                            <from>Jani</from>
                        </note>
                        """;

        //Act
        var result = _markupParser.Parse(value);

        //Assert
        result.Should().BeEquivalentTo(new List<MetaString>
            {
                new()
                {
                    Text = "Tove",
                    Tags = new List<MarkupTag>
                    {
                        new() { Name = "note", Kind = TagKind.Opening, Attributes = new ReadOnlyList<MarkupParameter>(new MarkupParameter { Name = "type", Value = "some thing or another" })},
                        new() { Name = "to", Kind = TagKind.Opening },
                    }
                },
                new()
                {
                    Text = "Jani",
                    Tags = new List<MarkupTag>
                    {
                        new() { Name = "note", Kind = TagKind.Opening, Attributes = new ReadOnlyList<MarkupParameter>(new MarkupParameter { Name = "type", Value = "some thing or another" })},
                        new() { Name = "from", Kind = TagKind.Opening },
                    }
                }
            });
        _terminal.Write($"{nameof(SpacesInsideAttributesSurroundedByQuotes)} : Passed!");
    }

    //TODO Disallow using unescaped tag brackets inside tags (Ex : <color red=255 <something else>>)
    private void UnescapedTagBracketsInsideTags()
    {
        //Arrange

        //Act

        //Assert
    }

    //TODO Escapable brackets (whatever the brackets used... possibly have escape characters defined in the specs class)
    private void EscapableBrackets()
    {
        //Arrange

        //Act

        //Assert
    }

    //TODO Support single self-closing tags ex : <br />
    private void SelfClosingTags()
    {
        //Arrange

        //Act

        //Assert
    }
}