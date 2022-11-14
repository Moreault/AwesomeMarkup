using ToolBX.Eloquentest.Integration;

namespace AwesomeMarkup.Integration;

[TestClass]
public class MarkupParserTest
{
    [TestClass]
    public class Parse : IntegrationTester<MarkupParser>
    {
        //TODO Fix this case (it's probably because of <Main> somewhere towards the end)
        //TODO Or throw something better than null/sequence contains no element or something
        [TestMethod]
        [Ignore]
        public void WhenStackTrace_DoNotThrow()
        {
            //Arrange
            var value = "   at ToolBX.AwesomeMarkup.Conversion.MarkupParameterConverter.Convert(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.AwesomeMarkup.Conversion.MarkupTagConverter.Convert(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.AwesomeMarkup.Conversion.MarkupExtractor.Extract(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.AwesomeMarkup.Parsing.MarkupParser.Parse(String value, MarkupLanguageSpecifications specifications)\r\n   at ToolBX.DML.NET.DmlSerializer.Deserialize(String text)\r\n   at ToolBX.MisterTerminal.DmlAnsiConverter.Convert(String text)\r\n   at ToolBX.MisterTerminal.TerminalWriter.WriteWithoutBreakingLine(String text, Object[] args)\r\n   at ToolBX.MisterTerminal.TerminalWriter.Write(String text, Object[] args)\r\n   at RoughConverter.Startup.Run(IServiceProvider serviceProvider) in C:\\Users\\seran\\source\\repos\\RoughConverter\\RoughConverter\\Startup.cs:line 46\r\n   at ToolBX.AssemblyInitializer.Console.ConsoleHost.UseStartup[T]()\r\n   at Program.<Main>$(String[] args) in C:\\Users\\seran\\source\\repos\\RoughConverter\\RoughConverter\\Program.cs:line 1";

            //Act
            var result = Instance.Parse(value);

            //Assert
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
                new MetaString
                {
                    Text = "test string "
                },
                new MetaString
                {
                    Text = "je suis rouge",
                    Tags = new List<MarkupTag>
                    {
                        new MarkupTag
                        {
                            Name = "markup",
                            Attributes = new List<MarkupParameter>
                            {
                                new MarkupParameter
                                {
                                    Name = "schema"
                                },
                                new MarkupParameter
                                {
                                    Name = "xmlns:xs",
                                    Value = "http://www.wjs.cum/markupSchema/"
                                },
                                new MarkupParameter
                                {
                                    Name = "version",
                                    Value = "1.0"
                                }
                            }
                        },
                        new MarkupTag
                        {
                            Name = "text",
                            Attributes = new List<MarkupParameter>
                            {
                                new MarkupParameter
                                {
                                    Name = "is-text",
                                    Value = "true"
                                }
                            },

                        },
                        new MarkupTag
                        {
                            Name = "div"
                        },
                        new MarkupTag
                        {
                            Name = "span",
                            Attributes = new List<MarkupParameter>
                            {
                                new MarkupParameter
                                {
                                    Name = "r",
                                    Value = "255"
                                },
                                new MarkupParameter
                                {
                                    Name = "g",
                                    Value = "0"
                                },
                                new MarkupParameter
                                {
                                    Name = "b",
                                    Value = "0"
                                },
                            }
                        },
                    }
                },
                new MetaString
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
                new MetaString
                {
                    Text = "The base is located near "
                },
                new MetaString
                {
                    Text = "Behabad",
                    Tags = new List<MarkupTag>
                    {
                        new MarkupTag
                        {
                            Name = "bold"
                        },
                        new MarkupTag
                        {
                            Name = "color",
                            Value = "#252321"
                        }
                    }
                },
                new MetaString
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
                new MetaString
                {
                    Text = "The base is located near "
                },
                new MetaString
                {
                    Text = "Behabad",
                    Tags = new List<MarkupTag>
                    {
                        new MarkupTag
                        {
                            Name = "color",
                            Value = "#252321"
                        }
                    }
                },
                new MetaString
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
                new MetaString
                {
                    Text = "That \"base\" is on the "
                },
                new MetaString
                {
                    Text = "outskirts",
                    Tags = new List<MarkupTag>
                    {
                        new MarkupTag
                        {
                            Name = "color",
                            Attributes = new List<MarkupParameter>
                            {
                                new MarkupParameter { Name = "red", Value = "164" },
                                new MarkupParameter { Name = "green", Value = "139" },
                                new MarkupParameter { Name = "blue", Value = "55" }
                            }
                        }
                    }
                },
                new MetaString
                {
                    Text = " of "
                },
                new MetaString
                {
                    Text = "Behabad",
                    Tags = new List<MarkupTag>
                    {
                        new MarkupTag
                        {
                            Name = "color",
                            Attributes = new List<MarkupParameter>
                            {
                                new MarkupParameter { Name = "red", Value = "56" },
                                new MarkupParameter { Name = "green", Value = "116" },
                                new MarkupParameter { Name = "blue", Value = "168" }
                            }
                        }
                    }
                },
                new MetaString
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
                new MetaString
                {
                    Text = "Double nested",
                    Tags = new List<MarkupTag>
                    {
                        new MarkupTag
                        {
                            Name = "a",
                            Value = "1"
                        },
                        new MarkupTag
                        {
                            Name = "a",
                            Value = "2"
                        }
                    }
                },
                new MetaString
                {
                    Text = "Unnested",
                    Tags = new List<MarkupTag>
                    {
                        new MarkupTag
                        {
                            Name = "a",
                            Value = "3"
                        }
                    }
                }
            });
        }

        //TODO Support "processing" tags (ex : <?version?>)
        [TestMethod]
        public void ProcessingTags()
        {
            //Arrange
            var value = """
                <?xml version="1.0" encoding="UTF-8"?>
                <note>
                  <to>Tove</to>
                  <from>Jani</from>
                  <heading>Reminder</heading>
                  <body>Don't forget me this weekend!</body>
                </note>
                """;

            //Act
            var result = Instance.Parse(value);

            //Assert
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