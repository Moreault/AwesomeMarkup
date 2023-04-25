using ToolBX.AwesomeMarkup.Resources;

namespace ToolBX.AwesomeMarkup.Tests.Conversion;

[TestClass]
public class StringExtensionsTester
{
    [TestClass]
    public class SplitWithQuotes : Tester
    {
        [TestMethod]
        public void WhenValueIsNull_Throw()
        {
            //Arrange
            string value = null!;
            var separator = Fixture.Create<char>();

            //Act
            var action = () => value.SplitWithQuotes(separator);

            //Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(value));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public void WhenValueIsEmpty_ReturnEmpty(string value)
        {
            //Arrange
            var separator = Fixture.Create<char>();

            //Act
            var result = value.SplitWithQuotes(separator);

            //Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void WhenContainsOddNumberOfSingleQuotes_Throw()
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
        public void WhenContainsOddNumberOfDoubleQuotes_Throw()
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
        public void WhenContainsNoQuotes_SplitNormally()
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
        public void WhenContainsAnEvenAmountOfSingleQuotes_SplitByIgnoringSeparatorsBetweenQuotes()
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
        public void WhenContainsAnEvenAmountOfDoubleQuotes_SplitByIgnoringSeparatorsBetweenQuotes()
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
}