namespace ToolBX.AwesomeMarkup.Tests.Parsing;

public class MarkupParserTest
{
    [TestClass]
    public class Parse
    {
        private readonly IMarkupParser _parser = new MarkupParser();

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void WhenValueIsNullOrEmpty_ThrowArgumentNullException(string value)
        {
            //Act
            var action = () => _parser.Parse(value);

            //Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow("   ")]
        public void WhenValueIsWhitespace_ThrowArgumentException(string value)
        {
            //Act
            var action = () => _parser.Parse(value);

            //Assert
            action.Should().Throw<ArgumentException>().WithMessage($"{Exceptions.ValueCannotBeWhitespace}*");
        }

        [TestMethod]
        public void WhenThereIsNoMarkup_ReturnSingleTextMetaString()
        {
            //Act
            var result = _parser.Parse("Just some plain text.");

            //Assert
            result.Should().BeEquivalentTo(new[] { new MetaString { Text = "Just some plain text." } });
        }

        [TestMethod]
        public void WhenTextContainsLiteralClosingBracket_TreatItAsText()
        {
            //Act
            var result = _parser.Parse("a > b <x>c</x>");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString { Text = "a > b " },
                new MetaString { Text = "c", Tags = [new MarkupTag { Name = "x", Kind = TagKind.Opening }] }
            });
        }

        [TestMethod]
        public void WhenOpeningTagIsNeverClosed_Throw()
        {
            //Arrange
            var value = "   at ToolBX.AwesomeMarkup.Conversion.MarkupParameterConverter.Convert(String value)\r\n   at Program.<Main>$(String[] args) in C:\\Users\\seran\\source\\repos\\RoughConverter\\Program.cs:line 1";

            //Act
            var action = () => _parser.Parse(value);

            //Assert
            action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.OpeningTagWithoutClosingTag, "Main")}");
        }

        [TestMethod]
        public void WhenSimpleColorTagWithValue_ReturnNestedText()
        {
            //Act
            var result = _parser.Parse("The base is located near <color=#252321>Behabad</color>.");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString { Text = "The base is located near " },
                new MetaString
                {
                    Text = "Behabad",
                    Tags = [new MarkupTag { Name = "color", Value = "#252321", Kind = TagKind.Opening }]
                },
                new MetaString { Text = "." }
            });
        }

        [TestMethod]
        public void WhenNestedTags_FlattenAncestorTagsOntoText()
        {
            //Act
            var result = _parser.Parse("The base is located near <bold><color=#252321>Behabad</color></bold>.");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString { Text = "The base is located near " },
                new MetaString
                {
                    Text = "Behabad",
                    Tags =
                    [
                        new MarkupTag { Name = "bold", Kind = TagKind.Opening },
                        new MarkupTag { Name = "color", Value = "#252321", Kind = TagKind.Opening }
                    ]
                },
                new MetaString { Text = "." }
            });
        }

        [TestMethod]
        public void WhenMultipleColorTagsWithAttributes_ReturnEachWithItsAttributes()
        {
            //Act
            var result = _parser.Parse("That \"base\" is on the <color red=164 green=139 blue=55>outskirts</color> of <color red=56 green=116 blue=168>Behabad</color>.");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString { Text = "That \"base\" is on the " },
                new MetaString
                {
                    Text = "outskirts",
                    Tags =
                    [
                        new MarkupTag
                        {
                            Name = "color",
                            Kind = TagKind.Opening,
                            Attributes =
                            [
                                new MarkupParameter { Name = "red", Value = "164" },
                                new MarkupParameter { Name = "green", Value = "139" },
                                new MarkupParameter { Name = "blue", Value = "55" }
                            ]
                        }
                    ]
                },
                new MetaString { Text = " of " },
                new MetaString
                {
                    Text = "Behabad",
                    Tags =
                    [
                        new MarkupTag
                        {
                            Name = "color",
                            Kind = TagKind.Opening,
                            Attributes =
                            [
                                new MarkupParameter { Name = "red", Value = "56" },
                                new MarkupParameter { Name = "green", Value = "116" },
                                new MarkupParameter { Name = "blue", Value = "168" }
                            ]
                        }
                    ]
                },
                new MetaString { Text = "." }
            });
        }

        [TestMethod]
        public void WhenNestedAndConsecutiveTagsOfSameName_LinkThemProperly()
        {
            //Act
            var result = _parser.Parse("<a=1><a=2>Double nested</a></a><a=3>Unnested</a>");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString
                {
                    Text = "Double nested",
                    Tags =
                    [
                        new MarkupTag { Name = "a", Value = "1", Kind = TagKind.Opening },
                        new MarkupTag { Name = "a", Value = "2", Kind = TagKind.Opening }
                    ]
                },
                new MetaString
                {
                    Text = "Unnested",
                    Tags = [new MarkupTag { Name = "a", Value = "3", Kind = TagKind.Opening }]
                }
            });
        }

        [TestMethod]
        public void WhenProcessingTag_EmitItAloneAndDoNotWrapFollowingContent()
        {
            //Act
            var result = _parser.Parse("""<?xml version="1.0" encoding="UTF-8"?><note><to>Tove</to><from>Jani</from></note>""");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString
                {
                    Tags =
                    [
                        new MarkupTag
                        {
                            Name = "xml",
                            Kind = TagKind.Processing,
                            Attributes =
                            [
                                new MarkupParameter { Name = "version", Value = "1.0" },
                                new MarkupParameter { Name = "encoding", Value = "UTF-8" }
                            ]
                        }
                    ]
                },
                new MetaString
                {
                    Text = "Tove",
                    Tags =
                    [
                        new MarkupTag { Name = "note", Kind = TagKind.Opening },
                        new MarkupTag { Name = "to", Kind = TagKind.Opening }
                    ]
                },
                new MetaString
                {
                    Text = "Jani",
                    Tags =
                    [
                        new MarkupTag { Name = "note", Kind = TagKind.Opening },
                        new MarkupTag { Name = "from", Kind = TagKind.Opening }
                    ]
                }
            });
        }

        [TestMethod]
        public void WhenLineBreaksAndSpacesBetweenTags_IgnoreWhitespaceOnlyRuns()
        {
            //Act
            var result = _parser.Parse("""
                                        <note>
                                            <to>Tove</to>
                                            <from>Jani</from>
                                        </note>
                                        """);

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString
                {
                    Text = "Tove",
                    Tags =
                    [
                        new MarkupTag { Name = "note", Kind = TagKind.Opening },
                        new MarkupTag { Name = "to", Kind = TagKind.Opening }
                    ]
                },
                new MetaString
                {
                    Text = "Jani",
                    Tags =
                    [
                        new MarkupTag { Name = "note", Kind = TagKind.Opening },
                        new MarkupTag { Name = "from", Kind = TagKind.Opening }
                    ]
                }
            });
        }

        [TestMethod]
        public void WhenAttributeValueIsQuotedAndContainsSpaces_KeepItAsASingleValue()
        {
            //Act
            var result = _parser.Parse("""
                                        <note type="some thing or another">
                                            <to>Tove</to>
                                        </note>
                                        """);

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString
                {
                    Text = "Tove",
                    Tags =
                    [
                        new MarkupTag
                        {
                            Name = "note",
                            Kind = TagKind.Opening,
                            Attributes = [new MarkupParameter { Name = "type", Value = "some thing or another" }]
                        },
                        new MarkupTag { Name = "to", Kind = TagKind.Opening }
                    ]
                }
            });
        }

        [TestMethod]
        public void WhenAttributeValueIsSingleQuoted_StripTheQuotes()
        {
            //Act
            var result = _parser.Parse("<color red='255'>x</color>");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString
                {
                    Text = "x",
                    Tags =
                    [
                        new MarkupTag
                        {
                            Name = "color",
                            Kind = TagKind.Opening,
                            Attributes = [new MarkupParameter { Name = "red", Value = "255" }]
                        }
                    ]
                }
            });
        }

        [TestMethod]
        public void WhenXmlLikeMarkupWithoutProcessingTags_ParseEntireTree()
        {
            //Arrange
            var value = "test string <markup schema xmlns:xs=\"http://www.wjs.cum/markupSchema/\" version=\"1.0\"><text is-text=\"true\"><div><span r=\"255\" g=\"0\" b=\"0\">je suis rouge</span></div></text></markup> je ne suis plus rouge";

            //Act
            var result = _parser.Parse(value);

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString { Text = "test string " },
                new MetaString
                {
                    Text = "je suis rouge",
                    Tags =
                    [
                        new MarkupTag
                        {
                            Name = "markup",
                            Kind = TagKind.Opening,
                            Attributes =
                            [
                                new MarkupParameter { Name = "schema" },
                                new MarkupParameter { Name = "xmlns:xs", Value = "http://www.wjs.cum/markupSchema/" },
                                new MarkupParameter { Name = "version", Value = "1.0" }
                            ]
                        },
                        new MarkupTag
                        {
                            Name = "text",
                            Kind = TagKind.Opening,
                            Attributes = [new MarkupParameter { Name = "is-text", Value = "true" }]
                        },
                        new MarkupTag { Name = "div", Kind = TagKind.Opening },
                        new MarkupTag
                        {
                            Name = "span",
                            Kind = TagKind.Opening,
                            Attributes =
                            [
                                new MarkupParameter { Name = "r", Value = "255" },
                                new MarkupParameter { Name = "g", Value = "0" },
                                new MarkupParameter { Name = "b", Value = "0" }
                            ]
                        }
                    ]
                },
                new MetaString { Text = " je ne suis plus rouge" }
            });
        }

        [TestMethod]
        public void WhenSelfClosingTag_EmitItAloneAndDoNotWrapFollowingText()
        {
            //Act
            var result = _parser.Parse("a<br/>b");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString { Text = "a" },
                new MetaString { Tags = [new MarkupTag { Name = "br", Kind = TagKind.SelfClosing }] },
                new MetaString { Text = "b" }
            });
        }

        [TestMethod]
        public void WhenSelfClosingTagHasSpaceBeforeSlash_StillRecognizeIt()
        {
            //Act
            var result = _parser.Parse("a<br />b");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString { Text = "a" },
                new MetaString { Tags = [new MarkupTag { Name = "br", Kind = TagKind.SelfClosing }] },
                new MetaString { Text = "b" }
            });
        }

        [TestMethod]
        public void WhenEmptyElement_EmitTaglessMetaStringForIt()
        {
            //Act
            var result = _parser.Parse("<b></b>");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString { Tags = [new MarkupTag { Name = "b", Kind = TagKind.Opening }] }
            });
        }

        [TestMethod]
        public void WhenEscapingIsEnabledAndBracketsAreEscaped_TreatThemAsLiteralText()
        {
            //Arrange
            var specifications = MarkupLanguageSpecifications.Dml with { EscapeCharacter = '\\' };

            //Act
            var result = _parser.Parse(@"\<not a tag\>", specifications);

            //Assert
            result.Should().BeEquivalentTo(new[] { new MetaString { Text = "<not a tag>" } });
        }

        [TestMethod]
        public void WhenEscapingIsDisabledAndBracketsAppear_TreatThemAsMarkup()
        {
            //Act
            var result = _parser.Parse(@"<bold>hi</bold>");

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                new MetaString { Text = "hi", Tags = [new MarkupTag { Name = "bold", Kind = TagKind.Opening }] }
            });
        }

        [TestMethod]
        public void WhenUnescapedBracketInsideTag_Throw()
        {
            //Act
            var action = () => _parser.Parse("<color red=255 <bold>>x</color>");

            //Assert
            action.Should().Throw<MarkupParsingException>();
        }

        [TestMethod]
        public void WhenClosingTagHasNoMatchingOpeningTag_Throw()
        {
            //Act
            var action = () => _parser.Parse("</color>");

            //Assert
            action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {Exceptions.UnmatchedClosingTag}");
        }

        [TestMethod]
        public void WhenClosingTagDoesNotMatchOpenTag_Throw()
        {
            //Act
            var action = () => _parser.Parse("<a>x</b>");

            //Assert
            action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.MismatchedClosingTag, "b", "a")}");
        }

        [TestMethod]
        public void WhenTagHasTooManyAssignationSymbols_Throw()
        {
            //Act
            var action = () => _parser.Parse("<color=a=b>x</color>");

            //Assert
            action.Should().Throw<MarkupParsingException>();
        }

        [TestMethod]
        public void WhenTagHasBothOpeningAndClosingSlashes_Throw()
        {
            //Act
            var action = () => _parser.Parse("</to/>");

            //Assert
            action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {Exceptions.ContainsSelfClosingAndClosingSlashes}");
        }

        [TestMethod]
        public void WhenMetaStringsAreReturned_PreserveDocumentOrder()
        {
            //Act
            var result = _parser.Parse("one<b>two</b>three");

            //Assert
            result.Should().ContainInOrder(
                new MetaString { Text = "one" },
                new MetaString { Text = "two", Tags = [new MarkupTag { Name = "b", Kind = TagKind.Opening }] },
                new MetaString { Text = "three" });
        }
    }
}
