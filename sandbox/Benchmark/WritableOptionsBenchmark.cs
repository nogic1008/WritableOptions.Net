#nullable disable
using BenchmarkDotNet.Attributes;

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

    private Awesome.Net.WritableOptions.WritableOptions<SampleOption> _awesomeWritableOptions;
    private Nogic.WritableOptions.JsonWritableOptions<SampleOption> _myWritableOptions;
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
        _myWritableOptions = new(_jsonFilePath, nameof(SampleOption), null!, null);
    }

    [Benchmark(Baseline = true)]
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

    [Benchmark]
    public void MyWritableOptions_Update()
    {
        for (int i = 0; i < 1000; i++)
        {
            _myWritableOptions.Update(_option);
        }
    }

    [IterationCleanup]
    public void Teardown()
    {
        if (File.Exists(_jsonFilePath))
            File.Delete(_jsonFilePath);
    }
}
