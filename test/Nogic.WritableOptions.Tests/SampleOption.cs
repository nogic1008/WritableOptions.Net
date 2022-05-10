namespace Nogic.WritableOptions.Tests;

/// <summary>Sample entity class for testing.</summary>
public class SampleOption
{
    /// <summary>Program last launched</summary>
    public DateTime LastLaunchedAt { get; set; }

    /// <summary>Retry interval</summary>
    public int Interval { get; set; }

    /// <summary>Connection string for DB</summary>
    public string? ConnectionString { get; set; }
}
