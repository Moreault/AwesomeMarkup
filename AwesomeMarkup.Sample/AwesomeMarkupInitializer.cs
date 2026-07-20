using ToolBX.AssemblyInitializer;
using ToolBX.AwesomeMarkup;

namespace AwesomeMarkup.Sample;

/// <summary>
/// Registers AwesomeMarkup's services explicitly. AwesomeMarkup no longer participates in AutoInject's
/// assembly scanning, so consumers register it through <see cref="ServiceCollectionExtensions.AddAwesomeMarkup"/>.
/// </summary>
public sealed class AwesomeMarkupInitializer : IAssemblyInitializer
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration) => services.AddAwesomeMarkup();

    public void Configure(IInitializerContext context) { }
}
