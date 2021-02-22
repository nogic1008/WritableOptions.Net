using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Nogic.WritableOptions
{
    public static class ServiceCollectionExtension
    {
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var environment = provider.GetRequiredService<IHostEnvironment>();
                var options = provider.GetRequiredService<IOptionsMonitor<T>>();
                return new JsonWritableOptions<T>(environment, options, section.Key, file);
            });
        }
    }
}
