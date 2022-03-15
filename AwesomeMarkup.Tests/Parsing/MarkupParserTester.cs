namespace ToolBX.AwesomeMarkup.Tests.Parsing;

[TestClass]
public class MarkupParserTest
{
    [TestClass]
    public class Parse : Tester<MarkupParser>
    {
        //TODO Test
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        public void WhenValueIsEmpty_Throw(string value)
        {
            //Arrange

            //Act
            var action = () => Instance.Parse(value);

            //Assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}