namespace ToolBX.AwesomeMarkup.Tests.Parsing;

[TestClass]
public class MarkupParserTest
{
    [TestClass]
    public class Parse : Tester<MarkupParser>
    {
        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void WhenValueIsNullOrEmpty_ThrowArgumentNullException(string value)
        {
            //Arrange

            //Act
            var action = () => Instance.Parse(value);

            //Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow("   ")]
        public void WhenValueIsWhitespace_ThrowArgumentException(string value)
        {
            //Arrange

            //Act
            var action = () => Instance.Parse(value);

            //Assert
            action.Should().Throw<ArgumentException>().WithMessage($"{Exceptions.ValueCannotBeWhitespace}*");
        }
    }
}
