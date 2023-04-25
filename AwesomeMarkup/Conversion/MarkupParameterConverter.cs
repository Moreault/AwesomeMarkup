namespace ToolBX.AwesomeMarkup.Conversion;

public interface IMarkupParameterConverter
{
    IReadOnlyList<MarkupParameter> Convert(string value, MarkupLanguageSpecifications specifications);
}

[AutoInject]
public class MarkupParameterConverter : IMarkupParameterConverter
{
    //TODO Better exception messages
    public IReadOnlyList<MarkupParameter> Convert(string value, MarkupLanguageSpecifications specifications)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
        if (specifications == null) throw new ArgumentNullException(nameof(specifications));

        if (!specifications.Attributes.QuoteRules.Double && value.Contains("\""))
            throw new MarkupParsingException("Double quotes found in string but specifications disallow double quotes!");
        if (!specifications.Attributes.QuoteRules.Single && value.Contains("'"))
            throw new MarkupParsingException("Single quotes found in string but specifications disallow single quotes!");

        var words = value.SplitWithQuotes(specifications.Attributes.Separator).ToList();

        var parameters = new List<MarkupParameter>();
        foreach (var word in words)
        {
            var nameAndValue = word.Split(specifications.Attributes.Assignation);
            if (nameAndValue.Length > 2) throw new MarkupParsingException(string.Format(Exceptions.TooManyAssignationSymbols, word, value, specifications.Attributes.Assignation));
            var parameterName = nameAndValue.First();

            var parameterValue = nameAndValue.Length == 1 ? string.Empty : nameAndValue[1];
            if (!specifications.Attributes.QuoteRules.Quoteless && (parameterValue.Contains("\"") || parameterValue.Contains("'")))
                throw new MarkupParsingException("Attribute value is expected to be between quotes but it was not the case");

            parameters.Add(new MarkupParameter
            {
                Name = parameterName,
                Value = parameterValue.Trim('\"', '\'')
            });
        }

        return parameters;
    }
}