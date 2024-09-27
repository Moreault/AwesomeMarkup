namespace ToolBX.AwesomeMarkup.Tests.Conversion;

[TestClass]
public class StringExtensionsTester : Tester
{
    [TestMethod]
    public void SplitWithQuotes_WhenValueIsNull_Throw()
    {
        //Arrange
        string value = null!;
        var separator = Dummy.Create<char>();

        //Act
        var action = () => value.SplitWithQuotes(separator);

        //Assert
        action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(value));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void SplitWithQuotes_WhenValueIsEmpty_ReturnEmpty(string value)
    {
        //Arrange
        var separator = Dummy.Create<char>();

        //Act
        var result = value.SplitWithQuotes(separator);

        //Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void SplitWithQuotes_WhenContainsOddNumberOfSingleQuotes_Throw()
    {
        //Arrange
        var value = """person butt='round""";
        var separator = ' ';

        //Act
        var action = () => value.SplitWithQuotes(separator);

        //Assert
        action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.OddNumberOfQuotes, 1)}");
    }

    [TestMethod]
    public void SplitWithQuotes_WhenContainsOddNumberOfDoubleQuotes_Throw()
    {
        //Arrange
        var value = "player target=\"old\" class=engineer\"";
        var separator = ' ';

        //Act
        var action = () => value.SplitWithQuotes(separator);

        //Assert
        action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.OddNumberOfQuotes, 3)}");
    }

    [TestMethod]
    public void SplitWithQuotes_WhenContainsNoQuotes_SplitNormally()
    {
        //Arrange
        var value = "player target=old class=engineer";
        var separator = ' ';

        //Act
        var result = value.SplitWithQuotes(separator);

        //Assert
        result.Should().BeEquivalentTo(new List<string>
            {
                "player",
                "target=old",
                "class=engineer"
            });
    }

    [TestMethod]
    public void SplitWithQuotes_WhenContainsAnEvenAmountOfSingleQuotes_SplitByIgnoringSeparatorsBetweenQuotes()
    {
        //Arrange
        var value = "player target='old and grey' class='engineer first class'";
        var separator = ' ';

        //Act
        var result = value.SplitWithQuotes(separator);

        //Assert
        result.Should().BeEquivalentTo(new List<string>
            {
                "player",
                "target='old and grey'",
                "class='engineer first class'"
            });
    }

    [TestMethod]
    public void SplitWithQuotes_WhenContainsAnEvenAmountOfDoubleQuotes_SplitByIgnoringSeparatorsBetweenQuotes()
    {
        //Arrange
        var value = "player target=\"old and grey\" class=\"engineer first class\"";
        var separator = ' ';

        //Act
        var result = value.SplitWithQuotes(separator);

        //Assert
        result.Should().BeEquivalentTo(new List<string>
            {
                "player",
                "target=\"old and grey\"",
                "class=\"engineer first class\""
            });
    }
}