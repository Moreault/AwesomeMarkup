using ToolBX.Collections.ReadOnly;

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

        var startsWithProcessingCharacter = value.StartsWith('?');
        var endsWithProcessingCharacter = value.EndsWith('?');
        var startsWithClosingSlash = !startsWithProcessingCharacter && value.StartsWith('/');
        var endsWithClosingSlash = !endsWithProcessingCharacter && value.EndsWith('/');

        var kind  = TagKind.Opening;
        if (startsWithClosingSlash && endsWithClosingSlash)
            throw new MarkupParsingException(Exceptions.ContainsSelfClosingAndClosingSlashes);
        if (startsWithClosingSlash)
            kind = TagKind.Closing;
        else if (endsWithClosingSlash)
            kind = TagKind.SelfClosing;
        else if (startsWithProcessingCharacter && endsWithProcessingCharacter)
            kind = TagKind.Processing;

        value = value.Trim('/', '?');

        var parameters = _markupParameterConverter.Convert(value, specifications);
        if (!parameters.Any()) throw new MarkupParsingException(string.Format(Exceptions.StringDoesNotContainValidParameters, value));

        var tagParameters = parameters.First();

        return new MarkupTag
        {
            Name = tagParameters.Name,
            Value = tagParameters.Value,
            Attributes = parameters.WithoutAt(0),
            Kind = kind
        };
    }
}