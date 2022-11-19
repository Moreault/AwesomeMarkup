using ToolBX.AwesomeMarkup.Resources;

namespace ToolBX.AwesomeMarkup;

public class MarkupParsingException : Exception
{
    public MarkupParsingException(string message) : base($"{Exceptions.CannotParseString} : {message}") { }
}