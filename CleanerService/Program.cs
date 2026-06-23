using CleanerService;
using Microsoft.Extensions.Hosting;
var host = Host.CreateDefaultBuilder(args).
    ConfigureServices(services =>
    {
        services.AddHostedService<CleanService>();
    }).UseWindowsService(Options =>
    {
        Options.ServiceName = "Cleaner Service";
    }).Build();
await host.RunAsync();