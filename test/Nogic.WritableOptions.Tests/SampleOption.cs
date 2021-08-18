namespace Nogic.WritableOptions.Tests;

using System;

public class SampleOption
{
    public DateTime LastLaunchedAt { get; set; }
    public int Interval { get; set; }
    public string? ConnectionString { get; set; }
}
