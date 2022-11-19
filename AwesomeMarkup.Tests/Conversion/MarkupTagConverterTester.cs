using ToolBX.AwesomeMarkup.Resources;

namespace ToolBX.AwesomeMarkup.Tests.Conversion;

public class MarkupTagConverterTestBase : Tester<MarkupTagConverter>
{

}

[TestClass]
public class MarkupTagConverterTester
{
    [TestClass]
    public class Convert : MarkupTagConverterTestBase
    {
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        public void WhenValueIsEmpty_Throw(string value)
        {
            //Arrange

            //Act
            Action action = () => Instance.Convert(value, new MarkupLanguageSpecifications());

            //Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void WhenSpecificationsIsNull_Throw()
        {
            //Arrange
            var value = Fixture.Create<string>();
            MarkupLanguageSpecifications specifications = null;

            //Act
            Action action = () => Instance.Convert(value, specifications);

            //Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Always_UseFirstParameterAsMainTagParameterAndRemoveItFromTheRest()
        {
            //Arrange
            var value = Fixture.Create<string>();

            var parameters = Fixture.Create<List<MarkupParameter>>();
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
        public void WhenOnlyOneParameterIsReturned_ReturnWithoutExtraAttributes()
        {
            //Arrange
            var value = Fixture.Create<string>();

            var tagParameters = Fixture.Create<MarkupParameter>();
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
        public void WhenNoParameterIsReturned_Throw()
        {
            //Arrange
            var value = Fixture.Create<string>();

            GetMock<IMarkupParameterConverter>().Setup(x => x.Convert(value, new MarkupLanguageSpecifications())).Returns(new List<MarkupParameter>());

            //Act
            Action action = () => Instance.Convert(value, new MarkupLanguageSpecifications());

            //Assert
            action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.StringDoesNotContainValidParameters, value)}");
        }
    }
}