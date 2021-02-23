using System.Threading.Tasks;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using Nogic.WritableOptions;

namespace ConsoleApp
{
    internal static class Program
    {
        private static async Task Main(string[] args)
            => await Host.CreateDefaultBuilder()
                .ConfigureLogging(logging => logging.ReplaceToSimpleConsole())
                .ConfigureServices((context, services) =>
                {
                    // Load app settings
                    var config = context.Configuration;
                    services.ConfigureWritable<AppOption>(config.GetSection(context.HostingEnvironment.ApplicationName));
                })
                .RunConsoleAppFrameworkAsync<AppBase>(args)
                .ConfigureAwait(false);
    }
}
