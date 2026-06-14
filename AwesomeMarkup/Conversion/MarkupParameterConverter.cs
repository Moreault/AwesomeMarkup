namespace ToolBX.AwesomeMarkup.Conversion;

public interface IMarkupParameterConverter
{
    IReadOnlyList<MarkupParameter> Convert(string value, MarkupLanguageSpecifications specifications);
}

public class MarkupParameterConverter : IMarkupParameterConverter
{
    public IReadOnlyList<MarkupParameter> Convert(string value, MarkupLanguageSpecifications specifications)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw string.IsNullOrEmpty(value)
                ? new ArgumentNullException(nameof(value))
                : new ArgumentException(Exceptions.ValueCannotBeWhitespace, nameof(value));
        if (specifications == null) throw new ArgumentNullException(nameof(specifications));

        if (!specifications.Attributes.QuoteRules.Double && value.Contains("\""))
            throw new MarkupParsingException(Exceptions.DoubleQuotesDisallowed);
        if (!specifications.Attributes.QuoteRules.Single && value.Contains("'"))
            throw new MarkupParsingException(Exceptions.SingleQuotesDisallowed);

        var words = value.SplitWithQuotes(specifications.Attributes.Separator);

        var parameters = new List<MarkupParameter>();
        foreach (var word in words)
        {
            var nameAndValue = word.Split(specifications.Attributes.Assignation);
            if (nameAndValue.Length > 2) throw new MarkupParsingException(string.Format(Exceptions.TooManyAssignationSymbols, word, value, specifications.Attributes.Assignation));
            var parameterName = nameAndValue.First();

            var parameterValue = nameAndValue.Length == 1 ? string.Empty : nameAndValue[1];
            if (!specifications.Attributes.QuoteRules.Quoteless && !parameterValue.Contains("\"") && !parameterValue.Contains("'"))
                throw new MarkupParsingException(Exceptions.AttributeValueMustBeQuoted);

            parameters.Add(new MarkupParameter
            {
                Name = parameterName,
                Value = parameterValue.Trim('\"', '\'')
            });
        }

        return parameters;
    }
}
