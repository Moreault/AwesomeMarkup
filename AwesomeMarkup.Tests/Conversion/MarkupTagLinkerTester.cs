using System.Runtime.ExceptionServices;
using ToolBX.AwesomeMarkup.Resources;

namespace ToolBX.AwesomeMarkup.Tests.Conversion;

[TestClass]
public class MarkupTagLinkerTester
{
    [TestClass]
    public class Link : Tester<MarkupTagLinker>
    {
        //TODO Test
        [TestMethod]
        public void WhenMarkupTagInfoIsNull_Throw()
        {
            //Arrange
            IReadOnlyList<MarkupTagInfo> markupTagInfo = null!;

            //Act
            var action = () => Instance.Link(markupTagInfo);

            //Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(markupTagInfo));
        }

        [TestMethod]
        public void WhenContainsNoTag_ReturnEmpty()
        {
            //Arrange
            var markupTagInfo = Array.Empty<MarkupTagInfo>();

            //Act
            var result = Instance.Link(markupTagInfo);

            //Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void WhenContainsTagsThatAreNotClosed_Throw()
        {
            //Arrange
            var markupTagInfo = new List<MarkupTagInfo>
            {
                Fixture.Create<MarkupTagInfo>()
            };

            //Act
            var action = () => Instance.Link(markupTagInfo);

            //Assert
            action.Should().Throw<ArgumentException>().WithMessage(string.Format(Exceptions.OpeningTagWithoutClosingTag, markupTagInfo.Single().Tag.Name));
        }

        [TestMethod]
        public void WhenContainsClosingTagsButNoOpeningTags_Throw()
        {
            //Arrange

            //Act

            //Assert
        }

        [TestMethod]
        public void WhenOpeningTagsHaveClosingTags_ReturnLinked()
        {
            //Arrange

            //Act

            //Assert
        }

        [TestMethod]
        public void WhenContaisnSelfClosingTags_ReturnLinkedToSelf()
        {
            //Arrange

            //Act

            //Assert
        }

        [TestMethod]
        public void WhenContainsXmlProlog_ReturnAsSelfClosing()
        {
            //Arrange

            //Act

            //Assert
        }
    }
}