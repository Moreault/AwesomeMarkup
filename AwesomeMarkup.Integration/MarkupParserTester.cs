using ToolBX.AwesomeMarkup;
using ToolBX.AwesomeMarkup.Resources;
using ToolBX.Eloquentest.Integration;

namespace AwesomeMarkup.Integration;

[TestClass]
public class MarkupParserTest
{
    [TestClass]
    public class Parse : IntegrationTester<MarkupParser>
    {
        [TestMethod]
        public void WhenStackTrace_Throw()
        {
            //Arrange
            var value = "   at ToolBX.AwesomeMarkup.Conversion.MarkupParameterConverter.Convert(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.AwesomeMarkup.Conversion.MarkupTagConverter.Convert(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.AwesomeMarkup.Conversion.MarkupExtractor.Extract(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.AwesomeMarkup.Parsing.MarkupParser.Parse(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.DML.NET.DmlSerializer.Deserialize(String text)\r\n   at ToolBX.MisterTerminal.DmlAnsiConverter.Convert(String text)\r\n   at ToolBX.MisterTerminal.TerminalWriter.WriteWithoutBreakingLine(String text, Object[] args)\r\n   at ToolBX.MisterTerminal.TerminalWriter.Write(String text, Object[] args)\r\n   at RoughConverter.Startup.Run(IServiceProvider serviceProvider) in C:\\Users\\seran\\source\\repos\\RoughConverter\\RoughConverter\\Startup.cs:line 46\r\n   at ToolBX.AssemblyInitializer.Console.ConsoleHost.UseStartup[T]()\r\n   at Program.<Main>$(String[] args) in C:\\Users\\seran\\source\\repos\\RoughConverter\\RoughConverter\\Program.cs:line 1";

            //Act
            var action = () => Instance.Parse(value);

            //Assert
            action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.OpeningTagWithoutClosingTag, "Main")}");
        }


        [TestMethod]
        public void XmlWithoutProcessingTags()
        {
            //Arrange
            var value = "test string <markup schema xmlns:xs=\"http://www.wjs.cum/markupSchema/\" version=\"1.0\"><text is-text=\"true\"><div><span r=\"255\" g=\"0\" b=\"0\">je suis rouge</span></div></text></markup> je ne suis plus rouge";

            //Act
            var result = Instance.Parse(value);

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
        }

        [TestMethod]
        public void NestedTags()
        {
            //Arrange
            var value = "The base is located near <bold><color=#252321>Behabad</color></bold>.";

            //Act
            var result = Instance.Parse(value);

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
        }

        [TestMethod]
        public void SimpleColorTag()
        {
            //Arrange
            var value = "The base is located near <color=#252321>Behabad</color>.";

            //Act
            var result = Instance.Parse(value);

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
        }

        [TestMethod]
        public void MultipleColorTags()
        {
            //Arrange
            var value = "That \"base\" is on the <color red=164 green=139 blue=55>outskirts</color> of <color red=56 green=116 blue=168>Behabad</color>.";

            //Act
            var result = Instance.Parse(value);

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
        }

        [TestMethod]
        public void NestedAndConsecutiveTagsOfSameName()
        {
            //Arrange
            var value = "<a=1><a=2>Double nested</a></a><a=3>Unnested</a>";

            //Act
            var result = Instance.Parse(value);

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
        }

        [TestMethod]
        public void ProcessingTags()
        {
            //Arrange
            var value = """<?xml version="1.0" encoding="UTF-8"?><note><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>""";

            //Act
            var result = Instance.Parse(value);

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
        }

        [TestMethod]
        public void LineBreaksAndSpaces()
        {
            //Arrange
            var value = """
                        <note>
                            <to>Tove</to>
                            <from>Jani</from>
                        </note>
                        """;

            //Act
            var result = Instance.Parse(value);

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
        }

        //TODO Disallow using unescaped tag brackets inside tags
        [TestMethod]
        public void UnescapedTagBracketsInsideTags()
        {
            //Arrange

            //Act

            //Assert
        }

        //TODO Allow spaces (or whatever attribute separator) inside attribute values that are surrounded by quotes
        [TestMethod]
        public void SpacesInsideAttributesSurroundedByQuotes()
        {
            //Arrange

            //Act

            //Assert
        }

        //TODO Escapable brackets (whatever the brackets used... possibly have escape characters defined in the specs class)
        [TestMethod]
        public void EscapableBrackets()
        {
            //Arrange

            //Act

            //Assert
        }

        //TODO Support single self-closing tags ex : <br />
        [TestMethod]
        public void SelfClosingTags()
        {
            //Arrange

            //Act

            //Assert
        }
    }
}