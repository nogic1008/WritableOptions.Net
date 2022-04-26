using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace MauiExample;

public partial class AppOption
{
    public DateTime LastChanged { get; set; }
    public string? ApiKey { get; set; }
    public override string ToString()
        => $"{{ {nameof(LastChanged)}: {LastChanged}, {nameof(ApiKey)} : {ApiKey} }}";

    [LoggerMessage(0, LogLevel.Information, "{Name}: {Option}")]
    public static partial void LogInfomation(
        ILogger logger,
        AppOption option,
        [CallerArgumentExpression("option")] string? name = null);
}
