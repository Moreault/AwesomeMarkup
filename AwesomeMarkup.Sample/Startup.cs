namespace AwesomeMarkup.Sample;

public sealed class Startup : ConsoleStartup
{
    public Startup(IConfiguration configuration) : base(configuration)
    {
    }

    public override void Run(IServiceProvider serviceProvider)
    {
        var terminal = serviceProvider.GetRequiredService<ITerminal>();

        var testRunner = serviceProvider.GetRequiredService<ITestRunner>();

        terminal.AskChoice(new Choice("Run automated tests", testRunner.Run), new Choice("Quit", () => { }));
        terminal.WaitForAnyInput();
    }
}