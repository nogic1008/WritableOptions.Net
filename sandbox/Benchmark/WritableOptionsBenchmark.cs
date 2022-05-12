#nullable disable
extern alias dev;
extern alias release;
using BenchmarkDotNet.Attributes;
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
        _awesomeWritableOptions = new(_jsonFilePath, nameof(SampleOption), null!, null);
        _releaseWritableOptions = new(_jsonFilePath, nameof(SampleOption), null!, null);
        _devWritableOptions = new(_jsonFilePath, nameof(SampleOption), null!, null);
    }

    [Benchmark]
    public void AwesomeWritableOptions_Update()
    {
        for (int i = 0; i < 1000; i++)
        {
            _awesomeWritableOptions.Update(o =>
            {
                o.LastLaunchedAt = _option.LastLaunchedAt;
                o.StringSettings = _option.StringSettings;
                o.IntSettings = _option.IntSettings;
            });
        }
    }

    [Benchmark(Baseline = true)]
    public void ReleaseWritableOptions_Update()
    {
        for (int i = 0; i < 1000; i++)
            _releaseWritableOptions.Update(_option);
    }

    [Benchmark]
    public void DevWritableOptions_Update()
    {
        for (int i = 0; i < 1000; i++)
            _devWritableOptions.Update(_option);
    }

    [IterationCleanup]
    public void Teardown()
    {
        if (File.Exists(_jsonFilePath))
            File.Delete(_jsonFilePath);
    }
}
