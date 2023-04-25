using ToolBX.AwesomeMarkup.Resources;

namespace ToolBX.AwesomeMarkup.Tests.Conversion;

[TestClass]
public class MarkupParameterConverterTester
{
    [TestClass]
    public class Convert : Tester<MarkupParameterConverter>
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
            MarkupLanguageSpecifications specifications = null!;

            //Act
            Action action = () => Instance.Convert(value, specifications);

            //Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void WhenOneWordHasMoreThanOneEqualSign_Throw()
        {
            //Arrange
            var value = "IsEnabled=true=probably IsEngineer=Nope IsSeb=Maybe";

            //Act
            Action action = () => Instance.Convert(value, new MarkupLanguageSpecifications());

            //Assert
            action.Should().Throw<MarkupParsingException>().WithMessage($"{Exceptions.CannotParseString} : {string.Format(Exceptions.TooManyAssignationSymbols, "IsEnabled=true=probably", value, "=")}");
        }

        [TestMethod]
        public void WhenOnlyContainsOneWordWithNoEqualsSignForValue_ReturnSingleParameterWithoutValue()
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
        public void WhenMultipleWordsHaveNoValue_ReturnValueLessParameters()
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
        public void WhenMultipleWordsWithValues_ReturnThat()
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

}