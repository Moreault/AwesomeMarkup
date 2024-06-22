namespace ToolBX.AwesomeMarkup.Tests.Conversion;

[TestClass]
public class MarkupTagLinkerTester : Tester<MarkupTagLinker>
{
    [TestMethod]
    public void Link_WhenMarkupTagInfoIsNull_Throw()
    {
        //Arrange
        IReadOnlyList<MarkupTagInfo> markupTagInfo = null!;

        //Act
        var action = () => Instance.Link(markupTagInfo);

        //Assert
        action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(markupTagInfo));
    }

    [TestMethod]
    public void Link_WhenContainsNoTag_ReturnEmpty()
    {
        //Arrange
        var markupTagInfo = Array.Empty<MarkupTagInfo>();

        //Act
        var result = Instance.Link(markupTagInfo);

        //Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void Link_WhenContainsTagsThatAreNotClosed_Throw()
    {
        //Arrange
        var openingTag = Dummy.Build<MarkupTag>().With(x => x.Kind, TagKind.Opening).Create();

        var markupTagInfo = new List<MarkupTagInfo>
        {
            Dummy.Build<MarkupTagInfo>().With(x => x.Tag, openingTag).Create()
        };

        //Act
        var action = () => Instance.Link(markupTagInfo);

        //Assert
        action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.OpeningTagWithoutClosingTag, markupTagInfo.Single().Tag.Name)}");
    }

    [TestMethod]
    public void Link_WhenOpeningTagsHaveClosingTags_ReturnLinkedByOpeningStartIndex()
    {
        //Arrange
        var openingTags = Dummy.Build<MarkupTagInfo>().With(x => x.Tag, () => Dummy.Build<MarkupTag>().With(y => y.Kind, TagKind.Opening).Create()).CreateMany(3).ToList();
        var closingTags = openingTags.Select(x => x with { Tag = x.Tag with { Value = string.Empty, Kind = TagKind.Closing, Attributes = new List<MarkupParameter>() }, StartIndex = x.EndIndex + 10, EndIndex = x.EndIndex + 20 }).ToList();

        var markupTagInfo = openingTags.Concat(closingTags).ToList();

        //Act
        var result = Instance.Link(markupTagInfo);

        //Assert
        result.Should().BeEquivalentTo(new List<LinkedTag>
        {
            new(openingTags[0], closingTags[0]),
            new(openingTags[1], closingTags[1]),
            new(openingTags[2], closingTags[2]),
        }.OrderBy(x => x.Opening.StartIndex));
    }

    [TestMethod]
    public void Link_WhenClosingTagsComeBeforeTheirOpeningTags_Throw()
    {
        //Arrange
        var openingTags = Dummy.Build<MarkupTagInfo>().With(x => x.Tag, () => Dummy.Build<MarkupTag>().With(y => y.Kind, TagKind.Opening).Create()).CreateMany(3).ToList();
        var closingTags = openingTags.Select(x => x with { Tag = x.Tag with { Value = string.Empty, Kind = TagKind.Closing, Attributes = new List<MarkupParameter>() }, StartIndex = x.EndIndex - 20, EndIndex = x.EndIndex - 10 }).ToList();

        var markupTagInfo = openingTags.Concat(closingTags).ToList();

        //Act
        var action = () => Instance.Link(markupTagInfo);

        //Assert
        action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.OpeningTagWithoutClosingTag, markupTagInfo.Last().Tag.Name)}");
    }

    [TestMethod]
    public void Link_WhenContainsSelfClosingTags_ReturnLinkedToSelf()
    {
        //Arrange
        var markupTagInfo = new List<MarkupTagInfo>
            {
                Dummy.Build<MarkupTagInfo>().With(x => x.Tag, Dummy.Build<MarkupTag>().With(y => y.Kind, TagKind.SelfClosing).Create()).Create()
            };

        //Act
        var result = Instance.Link(markupTagInfo);

        //Assert
        result.Should().BeEquivalentTo(new List<LinkedTag>
            {
                new(markupTagInfo.Single())
            });
    }

    [TestMethod]
    public void Link_WhenContainsProcessingTags_ReturnAsSelfClosing()
    {
        //Arrange
        var markupTagInfo = new List<MarkupTagInfo>
            {
                Dummy.Build<MarkupTagInfo>().With(x => x.Tag, Dummy.Build<MarkupTag>().With(y => y.Kind, TagKind.Processing).Create()).Create()
            };

        //Act
        var result = Instance.Link(markupTagInfo);

        //Assert
        result.Should().BeEquivalentTo(new List<LinkedTag>
            {
                new(markupTagInfo.Single())
            });
    }
}