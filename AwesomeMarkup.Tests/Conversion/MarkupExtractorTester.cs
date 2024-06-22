namespace ToolBX.AwesomeMarkup.Tests.Conversion;

[TestClass]
public class MarkupExtractorTester : Tester<MarkupExtractor>
{
    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow(null)]
    public void Extract_WhenValueIsEmpty_Throw(string value)
    {
        //Arrange
        var specifications = Dummy.Create<MarkupLanguageSpecifications>();

        //Act
        var action = () => Instance.Extract(value, specifications);

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Extract_WhenSpecificationsNull_Throw()
    {
        //Arrange
        var value = Dummy.Create<string>();
        MarkupLanguageSpecifications specifications = null!;

        //Act
        var action = () => Instance.Extract(value, specifications);

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Extract_WhenThereAreMoreOpeningBracketsThanClosingBrackets_Throw()
    {
        //Arrange
        var value = "There< > are a <few brackets here.";

        //Act
        var action = () => Instance.Extract(value, MarkupLanguageSpecifications.Dml);

        //Assert
        action.Should().Throw<Exception>().WithMessage($"Can't extract tags : There should be the same amount of opening and closing brackets in '{value}' but there are 2 and 1 respectively.");
    }

    [TestMethod]
    public void Extract_WhenThereAreMoreClosingBracketsThanOpeningBrackets_Throw()
    {
        //Arrange
        var value = "There< > are a >few brackets >here.";

        //Act
        var action = () => Instance.Extract(value, MarkupLanguageSpecifications.Dml);

        //Assert
        action.Should().Throw<Exception>().WithMessage($"Can't extract tags : There should be the same amount of opening and closing brackets in '{value}' but there are 1 and 3 respectively.");
    }

    [TestMethod]
    public void Extract_WhenThereIsTheSameNumberOfOpeningAndClosingBrackets_ReturnMarkupTagInfo()
    {
        //Arrange
        var value = "Some <color=red>string</color> with <bold>DML</bold> in it.";

        var tags = Dummy.CreateMany<MarkupTag>(4).ToList();
        GetMock<IMarkupTagConverter>().Setup(x => x.Convert("<color=red>", MarkupLanguageSpecifications.Dml)).Returns(tags[0]);
        GetMock<IMarkupTagConverter>().Setup(x => x.Convert("</color>", MarkupLanguageSpecifications.Dml)).Returns(tags[1]);
        GetMock<IMarkupTagConverter>().Setup(x => x.Convert("<bold>", MarkupLanguageSpecifications.Dml)).Returns(tags[2]);
        GetMock<IMarkupTagConverter>().Setup(x => x.Convert("</bold>", MarkupLanguageSpecifications.Dml)).Returns(tags[3]);

        //Act
        var result = Instance.Extract(value, MarkupLanguageSpecifications.Dml);

        //Assert
        result.Should().BeEquivalentTo(new List<MarkupTagInfo>
            {
                new()
                {
                    Tag = tags[0],
                    StartIndex = 5,
                    EndIndex = 15
                },
                new()
                {
                    Tag = tags[1],
                    StartIndex = 22,
                    EndIndex = 29
                },
                new()
                {
                    Tag = tags[2],
                    StartIndex = 36,
                    EndIndex = 41
                },
                new()
                {
                    Tag = tags[3],
                    StartIndex = 45,
                    EndIndex = 51
                },
            });
    }

    [TestMethod]
    public void Extract_WhenThereAreNoBrackets_ReturnEmpty()
    {
        //Arrange
        var value = "Some string with no brackets in it.";

        //Act
        var result = Instance.Extract(value, MarkupLanguageSpecifications.Dml);

        //Assert
        result.Should().BeEmpty();
    }
}