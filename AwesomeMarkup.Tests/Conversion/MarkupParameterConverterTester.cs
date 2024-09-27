namespace ToolBX.AwesomeMarkup.Tests.Conversion;

[TestClass]
public class MarkupParameterConverterTester : Tester<MarkupParameterConverter>
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
    public void Convert_WhenOneWordHasMoreThanOneEqualSign_Throw()
    {
        //Arrange
        var value = "IsEnabled=true=probably IsEngineer=Nope IsSeb=Maybe";

        //Act
        Action action = () => Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.TooManyAssignationSymbols, "IsEnabled=true=probably", value, "=")}");
    }

    [TestMethod]
    public void Convert_WhenOnlyContainsOneWordWithNoEqualsSignForValue_ReturnSingleParameterWithoutValue()
    {
        //Arrange
        var value = "IsEnabled";

        //Act
        var result = Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        result.Should().BeEquivalentTo(new List<MarkupParameter>
            {
                new MarkupParameter
                {
                    Name = "IsEnabled"
                }
            });
    }

    [TestMethod]
    public void Convert_WhenMultipleWordsHaveNoValue_ReturnValueLessParameters()
    {
        //Arrange
        var value = "IsEnabled IsEngineer IsSeb";

        //Act
        var result = Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        result.Should().BeEquivalentTo(new List<MarkupParameter>
            {

                new MarkupParameter
                {
                    Name = "IsEnabled"
                },
                new MarkupParameter
                {
                    Name = "IsEngineer"
                }, new MarkupParameter
                {
                    Name = "IsSeb"
                }
            });
    }

    [TestMethod]
    public void Convert_WhenMultipleWordsWithValues_ReturnThat()
    {
        //Arrange
        var value = "IsEnabled=true IsEngineer=Nope IsSeb=Maybe";

        //Act
        var result = Instance.Convert(value, new MarkupLanguageSpecifications());

        //Assert
        result.Should().BeEquivalentTo(new List<MarkupParameter>
            {

                new MarkupParameter
                {
                    Name = "IsEnabled",
                    Value = "true"
                },
                new MarkupParameter
                {
                    Name = "IsEngineer",
                    Value = "Nope"
                }, new MarkupParameter
                {
                    Name = "IsSeb",
                    Value = "Maybe"
                }
            });
    }

}