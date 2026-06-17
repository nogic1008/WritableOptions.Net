#nullable disable
extern alias dev;
extern alias release;
using Awesome.Net.WritableOptions.Extensions;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using AwesomeNet = Awesome.Net.WritableOptions;
using Dev = dev::Nogic.WritableOptions;
using Release = release::Nogic.WritableOptions;

namespace Benchmark;

[Config(typeof(BenchmarkConfig))]
public class WritableOptionsBenchmark
{
    public class SampleOption
    {
        public DateTime LastLaunchedAt { get; set; }
        public string[] StringSettings { get; set; }
        public int[] IntSettings { get; set; }
    }

    private const int UpdateLoopCount = 1000;

    private AwesomeNet.IWritableOptions<SampleOption> _awesomeWritableOptions;
    private Release.IWritableOptions<SampleOption> _releaseWritableOptions;
    private Dev.IWritableOptions<SampleOption> _devWritableOptions;
    private ServiceProvider _awesomeProvider;
    private ServiceProvider _releaseProvider;
    private ServiceProvider _devProvider;
    private SampleOption _option;
    private string _jsonFilePath;

    [GlobalSetup]
    public void GlobalSetup() =>
        _option = new()
        {
            LastLaunchedAt = DateTime.Now,
            StringSettings = Enumerable
                .Range(1, 100)
                .Select(static (_) => Guid.NewGuid().ToString())
                .ToArray(),
            IntSettings = Enumerable.Range(1, 100).ToArray(),
        };

    [IterationSetup]
    public void ItarationSetup()
    {
        _jsonFilePath = Path.GetTempFileName();
        File.AppendAllText(_jsonFilePath, "{}");

        string directoryPath = Path.GetDirectoryName(_jsonFilePath)!;
        string fileName = Path.GetFileName(_jsonFilePath);

        var awesomeConfiguration = CreateConfiguration(directoryPath, fileName);
        var awesomeServices = new ServiceCollection();
        awesomeServices.AddSingleton<IConfiguration>(awesomeConfiguration);
        awesomeServices.AddSingleton<IHostEnvironment>(new BenchmarkHostEnvironment(directoryPath));
        awesomeServices.ConfigureWritableOptions<SampleOption>(
            awesomeConfiguration,
            nameof(SampleOption),
            fileName
        );
        _awesomeProvider = awesomeServices.BuildServiceProvider();

        var releaseConfiguration = CreateConfiguration(directoryPath, fileName);
        var releaseServices = new ServiceCollection();
        releaseServices.AddSingleton<IConfiguration>(releaseConfiguration);
        releaseServices.AddSingleton<IHostEnvironment>(new BenchmarkHostEnvironment(directoryPath));
        Release.ServiceCollectionExtension.ConfigureWritable<SampleOption>(
            releaseServices,
            releaseConfiguration.GetSection(nameof(SampleOption)),
            fileName
        );
        _releaseProvider = releaseServices.BuildServiceProvider();

        var devConfiguration = CreateConfiguration(directoryPath, fileName);
        var devServices = new ServiceCollection();
        devServices.AddSingleton<IConfiguration>(devConfiguration);
        devServices.AddSingleton<IHostEnvironment>(new BenchmarkHostEnvironment(directoryPath));
        Dev.ServiceCollectionExtension.ConfigureWritable<SampleOption>(
            devServices,
            devConfiguration.GetSection(nameof(SampleOption)),
            fileName
        );
        _devProvider = devServices.BuildServiceProvider();

        _awesomeWritableOptions =
            _awesomeProvider.GetRequiredService<AwesomeNet.IWritableOptions<SampleOption>>();
        _releaseWritableOptions =
            _releaseProvider.GetRequiredService<Release.IWritableOptions<SampleOption>>();
        _devWritableOptions = _devProvider.GetRequiredService<Dev.IWritableOptions<SampleOption>>();
    }

    [Benchmark(
        Description = $"{nameof(Awesome)}.{nameof(Awesome.Net)}.{nameof(Awesome.Net.WritableOptions)}"
    )]
    public void AwesomeWritableOptions_Update()
    {
        for (int i = 0; i < UpdateLoopCount; i++)
        {
            _awesomeWritableOptions.Update(o =>
            {
                o.LastLaunchedAt = _option.LastLaunchedAt;
                o.StringSettings = _option.StringSettings;
                o.IntSettings = _option.IntSettings;
            });
        }
    }

    [Benchmark(
        Baseline = true,
        Description = $"(NuGet){nameof(release.Nogic)}.{nameof(release.Nogic.WritableOptions)}"
    )]
    public void ReleaseWritableOptions_Update()
    {
        for (int i = 0; i < UpdateLoopCount; i++)
            _releaseWritableOptions.Update(_option);
    }

    [Benchmark(Description = $"(Dev){nameof(dev.Nogic)}.{nameof(dev.Nogic.WritableOptions)}")]
    public void DevWritableOptions_Update()
    {
        for (int i = 0; i < UpdateLoopCount; i++)
            _devWritableOptions.Update(_option);
    }

    [IterationCleanup]
    public void Teardown()
    {
        _awesomeProvider?.Dispose();
        _releaseProvider?.Dispose();
        _devProvider?.Dispose();

        if (File.Exists(_jsonFilePath))
            File.Delete(_jsonFilePath);
    }

    private static IConfigurationRoot CreateConfiguration(string directoryPath, string fileName) =>
        new ConfigurationBuilder().SetBasePath(directoryPath).AddJsonFile(fileName).Build();

    private sealed class BenchmarkHostEnvironment(string contentRootPath) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Benchmark";

        public string ApplicationName { get; set; } = nameof(WritableOptionsBenchmark);

        public string ContentRootPath { get; set; } = contentRootPath;

        public IFileProvider ContentRootFileProvider { get; set; } =
            new PhysicalFileProvider(contentRootPath);
    }
}
