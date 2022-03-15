namespace ToolBX.AwesomeMarkup.Conversion;

public interface IMarkupParameterConverter
{
    IReadOnlyList<MarkupParameter> Convert(string value, MarkupLanguageSpecifications specifications);
}

[AutoInject]
public class MarkupParameterConverter : IMarkupParameterConverter
{
    public IReadOnlyList<MarkupParameter> Convert(string value, MarkupLanguageSpecifications specifications)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
        if (specifications == null) throw new ArgumentNullException(nameof(specifications));

        var words = value.Split(specifications.Attributes.Separator).ToList();

        var parameters = new List<MarkupParameter>();
        foreach (var word in words)
        {
            var nameAndValue = word.Split(specifications.Attributes.AssignationSeparator);
            if (nameAndValue.Length > 2) throw new Exception($"Can't convert string to {nameof(MarkupParameter)} : '{word}' in '{value}' has too many '{specifications.Attributes.AssignationSeparator}' signs.");
            var parameterName = nameAndValue.First();
            var parameterValue = nameAndValue.Length == 1 ? string.Empty : nameAndValue[1].Trim('\"', '\'');

            parameters.Add(new MarkupParameter
            {
                Name = parameterName,
                Value = parameterValue
            });
        }

        return parameters;
    }
}