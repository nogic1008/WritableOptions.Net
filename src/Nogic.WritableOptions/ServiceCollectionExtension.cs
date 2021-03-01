using System;
using System.IO;
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
                string jsonFilePath;

                var environment = provider.GetService<IHostEnvironment>();
                if (environment is not null)
                {
                    var fileProvider = environment.ContentRootFileProvider;
                    var fileInfo = fileProvider.GetFileInfo(file);
                    jsonFilePath = fileInfo.PhysicalPath;
                }
                else
                {
                    jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
                }

                var configuration = provider.GetService<IConfigurationRoot>();
                var options = provider.GetRequiredService<IOptionsMonitor<T>>();
                return new JsonWritableOptions<T>(jsonFilePath, section.Key, options, configuration);
            });
        }
    }
}
