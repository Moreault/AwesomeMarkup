namespace ToolBX.AwesomeMarkup.Conversion;

public interface IMarkupTagConverter
{
    MarkupTag Convert(string value, MarkupLanguageSpecifications specifications);
}

[AutoInject]
public class MarkupTagConverter : IMarkupTagConverter
{
    private readonly IMarkupParameterConverter _markupParameterConverter;

    public MarkupTagConverter(IMarkupParameterConverter markupParameterConverter)
    {
        _markupParameterConverter = markupParameterConverter;
    }

    public MarkupTag Convert(string value, MarkupLanguageSpecifications specifications)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
        if (specifications == null) throw new ArgumentNullException(nameof(specifications));
        value = value.Trim(specifications.Brackets.Opening, specifications.Brackets.Closing);

        var parameters = _markupParameterConverter.Convert(value, specifications).ToList();
        if (!parameters.Any()) throw new Exception($"Can't convert string to {nameof(MarkupTag)} : '{value}' does not contain any valid parameters.");

        var tagParameters = parameters.First();
        parameters.RemoveAt(0);

        return new MarkupTag
        {
            Name = tagParameters.Name,
            Value = tagParameters.Value,
            Attributes = parameters
        };
    }
}