namespace ToolBX.AwesomeMarkup;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services necessary to use AwesomeMarkup. Do not use if you are using AutoInject.
    /// </summary>
    public static IServiceCollection AddAwesomeMarkup(this IServiceCollection services)
    {
        return services
            .AddSingleton<IMarkupParameterConverter, MarkupParameterConverter>()
            .AddSingleton<IMarkupTagConverter, MarkupTagConverter>()
            .AddSingleton<IMarkupExtractor, MarkupExtractor>()
            .AddSingleton<IMarkupTagLinker, MarkupTagLinker>()
            .AddSingleton<IMarkupParser, MarkupParser>();
    }
}