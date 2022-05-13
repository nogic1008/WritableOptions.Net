#nullable disable
extern alias dev;
extern alias release;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Options;
using Moq;
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

    private const int UpdateLoopCount = 100;

    private IOptionsMonitor<SampleOption> _options;
    private AwesomeNet.WritableOptions<SampleOption> _awesomeWritableOptions;
    private Release.JsonWritableOptions<SampleOption> _releaseWritableOptions;
    private Dev.JsonWritableOptions<SampleOption> _devWritableOptions;
    private SampleOption _option;
    private string _jsonFilePath;

    [GlobalSetup]
    public void GlobalSetup() => _option = new()
    {
        LastLaunchedAt = DateTime.Now,
        StringSettings = Enumerable.Range(1, 100).Select((_) => Guid.NewGuid().ToString()).ToArray(),
        IntSettings = Enumerable.Range(1, 100).ToArray()
    };

    [IterationSetup]
    public void ItarationSetup()
    {
        _jsonFilePath = Path.GetTempFileName();
        File.AppendAllText(_jsonFilePath, "{}");
        _options = new Mock<IOptionsMonitor<SampleOption>>().Object;
        _awesomeWritableOptions = new(_jsonFilePath, nameof(SampleOption), _options, null);
        _releaseWritableOptions = new(_jsonFilePath, nameof(SampleOption), _options, null);
        _devWritableOptions = new(_jsonFilePath, nameof(SampleOption), _options, null);
    }

    [Benchmark(Description = $"{nameof(Awesome)}.{nameof(Awesome.Net)}.{nameof(Awesome.Net.WritableOptions)}")]
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

    [Benchmark(Baseline = true, Description = $"(NuGet){nameof(release.Nogic)}.{nameof(release.Nogic.WritableOptions)}")]
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
        if (File.Exists(_jsonFilePath))
            File.Delete(_jsonFilePath);
    }
}
