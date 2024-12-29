using System.Reflection;
using Microsoft.Extensions.Hosting;
using NetDaemon.Extensions.Logging;
using NetDaemon.Extensions.Scheduler;
using NetDaemon.Extensions.Tts;
using NetDaemon.Runtime;
using HomeAssistantGenerated;
using HomeAssistantApps;
using Microsoft.Extensions.Configuration;

#pragma warning disable CA1812

try
{
    await Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile("appsettings.docker.json", optional: true, reloadOnChange: true);
        })
        .UseNetDaemonAppSettings()
        .UseNetDaemonDefaultLogging()
        .UseNetDaemonRuntime()
        .UseNetDaemonTextToSpeech()
        .ConfigureServices((_, services) =>
            services
                .AddAppsFromAssembly(Assembly.GetExecutingAssembly())
                .AddNetDaemonStateManager()
                .AddNetDaemonScheduler()
                .AddNewMoonNotifier()
                .AddNetDaemonScheduler()
                .AddHomeAssistantGenerated()
        )
        .Build()
        .RunAsync()
        .ConfigureAwait(false);
}
catch (Exception e)
{
    Console.WriteLine($"Failed to start host... {e}");
    throw;
}