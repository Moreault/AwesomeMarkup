namespace ToolBX.AwesomeMarkup;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services necessary to use AwesomeMarkup. Registration is fully explicit and reflection-free, making it trimming/NativeAOT-safe.
    /// </summary>
    public static IServiceCollection AddAwesomeMarkup(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Add(new ServiceDescriptor(typeof(IMarkupParser), typeof(MarkupParser), lifetime));

        return services;
    }
}
