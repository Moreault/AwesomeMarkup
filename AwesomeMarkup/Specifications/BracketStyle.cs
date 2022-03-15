namespace ToolBX.AwesomeMarkup.Specifications;

public record BracketStyle
{
    public static BracketStyle Angle = new() { Opening = '<', Closing = '>' };
    public static BracketStyle Square = new() { Opening = '[', Closing = ']' };
    public static BracketStyle Curly = new() { Opening = '{', Closing = '}' };

    public char Opening { get; init; }
    public char Closing { get; init; }
}