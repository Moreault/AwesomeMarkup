namespace ToolBX.AwesomeMarkup.Tests.Parsing;

[TestClass]
public class ModelEqualityTester
{
    [TestMethod]
    public void MarkupParameter_WhenNamesAndValuesDifferOnlyByCase_AreEqualAndShareHashCode()
    {
        //Arrange
        var first = new MarkupParameter { Name = "Red", Value = "FF" };
        var second = new MarkupParameter { Name = "red", Value = "ff" };

        //Assert
        first.Equals(second).Should().BeTrue();
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [TestMethod]
    public void MarkupTag_WhenEqualByValueDespiteCaseAndDistinctAttributeLists_ShareHashCode()
    {
        //Arrange
        var first = new MarkupTag
        {
            Name = "Color",
            Value = "RED",
            Kind = TagKind.Opening,
            Attributes = [new MarkupParameter { Name = "Alpha", Value = "255" }]
        };
        var second = new MarkupTag
        {
            Name = "color",
            Value = "red",
            Kind = TagKind.Opening,
            Attributes = [new MarkupParameter { Name = "alpha", Value = "255" }]
        };

        //Assert
        first.Equals(second).Should().BeTrue();
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [TestMethod]
    public void MetaString_WhenEqualByValueWithDistinctTagLists_ShareHashCode()
    {
        //Arrange
        var first = new MetaString
        {
            Text = "hi",
            Tags = [new MarkupTag { Name = "Bold", Kind = TagKind.Opening }]
        };
        var second = new MetaString
        {
            Text = "hi",
            Tags = [new MarkupTag { Name = "bold", Kind = TagKind.Opening }]
        };

        //Assert
        first.Equals(second).Should().BeTrue();
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [TestMethod]
    public void MarkupTag_WhenUsedAsDictionaryKey_CaseInsensitiveLookupSucceeds()
    {
        //Arrange
        var dictionary = new Dictionary<MarkupTag, int>
        {
            [new MarkupTag { Name = "Color", Value = "RED", Kind = TagKind.Opening }] = 1
        };

        //Act
        var found = dictionary.TryGetValue(new MarkupTag { Name = "color", Value = "red", Kind = TagKind.Opening }, out var value);

        //Assert
        found.Should().BeTrue();
        value.Should().Be(1);
    }
}
