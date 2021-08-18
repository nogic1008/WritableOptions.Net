namespace ConsoleAppExample;

using System;

public class AppOption
{
    public DateTime LastLaunchedAt { get; set; }
    public string? ApiKey { get; set; }
    public override string ToString()
        => $"{{ {nameof(LastLaunchedAt)}: {LastLaunchedAt}, {nameof(ApiKey)} : {ApiKey} }}";
}
