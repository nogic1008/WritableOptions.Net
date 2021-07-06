using System;

namespace Nogic.WritableOptions.Tests
{
    public class SampleOption
    {
        public DateTime LastLaunchedAt { get; set; }
        public int Interval { get; set; }
        public string? ConnectionString { get; set; }
    }
}
