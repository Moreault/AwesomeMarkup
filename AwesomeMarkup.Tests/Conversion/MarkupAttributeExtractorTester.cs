namespace ToolBX.AwesomeMarkup.Tests.Conversion;

[TestClass]
public class MarkupAttributeExtractorTester : Tester<MarkupAttributeExtractor>
{
    [TestMethod]
    public void Extract_WhenValueIsNull_Throw()
    {
        //Arrange
        string value = null!;
        var specifications = new MarkupLanguageSpecifications();

        //Act
        var action = () => Instance.Extract(value, specifications);

        //Assert
        action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(value));
    }

    [TestMethod]
    public void Extract_WhenSpecificationsIsNull_Throw()
    {
        //Arrange
        var value = "some string";
        MarkupLanguageSpecifications specifications = null!;

        //Act
        var action = () => Instance.Extract(value, specifications);

        //Assert
        action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(specifications));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void Extract_WhenValueIsEmpty_ReturnEmpty(string value)
    {
        //Arrange
        var specifications = new MarkupLanguageSpecifications();

        //Act
        var result = Instance.Extract(value, specifications);

        //Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void Extract_WhenSingleAttribute_ReturnSingleElement()
    {
        //Arrange
        var value = "color=red";
        var specifications = new MarkupLanguageSpecifications();

        //Act
        var result = Instance.Extract(value, specifications);

        //Assert
        result.Should().BeEquivalentTo(new List<string> { "color=red" });
    }

    [TestMethod]
    public void Extract_WhenMultipleAttributes_ReturnAllAttributes()
    {
        //Arrange
        var value = "red=255 green=128 blue=0";
        var specifications = new MarkupLanguageSpecifications();

        //Act
        var result = Instance.Extract(value, specifications);

        //Assert
        result.Should().BeEquivalentTo(new List<string> { "red=255", "green=128", "blue=0" });
    }

    [TestMethod]
    public void Extract_WhenQuotedValuesContainSeparator_DoNotSplitInsideQuotes()
    {
        //Arrange
        var value = "type=\"some thing\" name=test";
        var specifications = new MarkupLanguageSpecifications();

        //Act
        var result = Instance.Extract(value, specifications);

        //Assert
        result.Should().BeEquivalentTo(new List<string> { "type=\"some thing\"", "name=test" });
    }

    [TestMethod]
    public void Extract_WhenOddNumberOfQuotes_Throw()
    {
        //Arrange
        var value = "type=\"broken name=test";
        var specifications = new MarkupLanguageSpecifications();

        //Act
        var action = () => Instance.Extract(value, specifications);

        //Assert
        action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.OddNumberOfQuotes, 1)}");
    }
}
