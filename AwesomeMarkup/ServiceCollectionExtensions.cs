namespace ToolBX.AwesomeMarkup;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services necessary to use AwesomeMarkup. Do not use if you are using AutoInject.
    /// </summary>
    public static IServiceCollection AddAwesomeMarkup(this IServiceCollection services, AutoInjectOptions? options = null)
    {
        return services.AddAutoInjectServices(Assembly.GetExecutingAssembly(), options);
    }
}