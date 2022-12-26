using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

#pragma warning disable CA1050, RCS1110
public partial class AppOption
{
    public DateTime LastChanged { get; set; }

    public string? ApiKey { get; set; }

    public override string ToString()
        => $"{{ {nameof(LastChanged)}: {LastChanged}, {nameof(ApiKey)} : {ApiKey} }}";

#if NET6_0_OR_GREATER
    [LoggerMessage(0, LogLevel.Information, "{Name}: {Option}")]
    public static partial void LogInfomation(
        ILogger logger,
        AppOption option,
        [CallerArgumentExpression("option")] string? name = null);
#endif
}
