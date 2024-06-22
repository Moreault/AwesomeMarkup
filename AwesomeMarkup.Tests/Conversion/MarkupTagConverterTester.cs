namespace ToolBX.AwesomeMarkup.Tests.Conversion;

[TestClass]
public class MarkupTagConverterTester : Tester<MarkupTagConverter>
{
    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow(null)]
    public void Convert_WhenValueIsEmpty_Throw(string value)
    {
        //Arrange

        //Act
        Action action = () => Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Convert_WhenSpecificationsIsNull_Throw()
    {
        //Arrange
        var value = Dummy.Create<string>();
        MarkupLanguageSpecifications specifications = null!;

        //Act
        Action action = () => Instance.Convert(value, specifications);

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Convert_Always_UseFirstParameterAsMainTagParameterAndRemoveItFromTheRest()
    {
        //Arrange
        var value = Dummy.Create<string>();

        var parameters = Dummy.Create<List<MarkupParameter>>();
        GetMock<IMarkupParameterConverter>().Setup(x => x.Convert(value, new MarkupLanguageSpecifications())).Returns(parameters);

        var tagParameters = parameters.First();
        var otherParameters = parameters.ToList();
        otherParameters.RemoveAt(0);

        //Act
        var result = Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        result.Should().BeEquivalentTo(new MarkupTag
        {
            Name = tagParameters.Name,
            Value = tagParameters.Value,
            Attributes = otherParameters,
            Kind = TagKind.Opening
        });
    }

    [TestMethod]
    public void Convert_WhenOnlyOneParameterIsReturned_ReturnWithoutExtraAttributes()
    {
        //Arrange
        var value = Dummy.Create<string>();

        var tagParameters = Dummy.Create<MarkupParameter>();
        GetMock<IMarkupParameterConverter>().Setup(x => x.Convert(value, new MarkupLanguageSpecifications())).Returns(new List<MarkupParameter> { tagParameters });

        //Act
        var result = Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        result.Should().BeEquivalentTo(new MarkupTag
        {
            Name = tagParameters.Name,
            Value = tagParameters.Value,
            Kind = TagKind.Opening
        });
    }

    [TestMethod]
    public void Convert_WhenNoParameterIsReturned_Throw()
    {
        //Arrange
        var value = Dummy.Create<string>();

        GetMock<IMarkupParameterConverter>().Setup(x => x.Convert(value, new MarkupLanguageSpecifications())).Returns(new List<MarkupParameter>());

        //Act
        Action action = () => Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.StringDoesNotContainValidParameters, value)}");
    }

    [TestMethod]
    public void Convert_WhenValueContainsSlashAtStartAndEnd_Throw()
    {
        //Arrange
        var value = $"/{Dummy.Create<string>()}/";

        //Act
        Action action = () => Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {Exceptions.ContainsSelfClosingAndClosingSlashes}");
    }

    [TestMethod]
    public void Convert_WhenValueContainsTagAtStart_ReturnAsClosingTag()
    {
        //Arrange
        var trimmedValue = Dummy.Create<string>();
        var value = $"/{trimmedValue}";

        var tagParameters = Dummy.Create<MarkupParameter>();
        GetMock<IMarkupParameterConverter>().Setup(x => x.Convert(trimmedValue, new MarkupLanguageSpecifications())).Returns(new List<MarkupParameter> { tagParameters });

        //Act
        var result = Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        result.Kind.Should().Be(TagKind.Closing);
    }

    [TestMethod]
    public void Convert_WhenValueContainsTagAtEnd_ReturnAsSelfClosingTag()
    {
        //Arrange
        var trimmedValue = Dummy.Create<string>();
        var value = $"{trimmedValue}/";

        var tagParameters = Dummy.Create<MarkupParameter>();
        GetMock<IMarkupParameterConverter>().Setup(x => x.Convert(trimmedValue, new MarkupLanguageSpecifications())).Returns(new List<MarkupParameter> { tagParameters });

        //Act
        var result = Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        result.Kind.Should().Be(TagKind.SelfClosing);
    }

    [TestMethod]
    public void Convert_WhenValueStartsAndEndWithProcessingCharacters_ReturnAsProcessingTag()
    {
        //Arrange
        var trimmedValue = Dummy.Create<string>();
        var value = $"?{trimmedValue}?";

        var tagParameters = Dummy.Create<MarkupParameter>();
        GetMock<IMarkupParameterConverter>().Setup(x => x.Convert(trimmedValue, new MarkupLanguageSpecifications())).Returns(new List<MarkupParameter> { tagParameters });

        //Act
        var result = Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        result.Kind.Should().Be(TagKind.Processing);
    }
}