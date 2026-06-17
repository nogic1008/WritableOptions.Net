using System.Buffers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace Nogic.WritableOptions;

internal sealed class JsonFileOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
    where TOptions : new()
{
    private readonly string _jsonFilePath;
    private readonly string _section;
    private readonly JsonSerializerOptions? _serializerOptions;
    private readonly object _gate = new();
    private readonly List<Action<TOptions, string?>> _listeners = [];

    public JsonFileOptionsMonitor(
        string jsonFilePath,
        string section,
        JsonSerializerOptions? serializerOptions
    )
    {
        _jsonFilePath = jsonFilePath;
        _section = section;
        _serializerOptions = serializerOptions;
    }

    public TOptions CurrentValue => ReadCurrentValue();

    public TOptions Get(string? name) => ReadCurrentValue();

    public IDisposable OnChange(Action<TOptions, string?> listener)
    {
        lock (_gate)
        {
            _listeners.Add(listener);
        }

        return new ListenerRegistration(this, listener);
    }

    public void NotifyChanged(TOptions changedValue)
    {
        Action<TOptions, string?>[] listeners;
        lock (_gate)
        {
            listeners = [.. _listeners];
        }

        foreach (var listener in listeners)
        {
            listener(changedValue, Options.DefaultName);
        }
    }

    private void RemoveListener(Action<TOptions, string?> listener)
    {
        lock (_gate)
        {
            _ = _listeners.Remove(listener);
        }
    }

    private TOptions ReadCurrentValue()
    {
        if (!File.Exists(_jsonFilePath))
            return new TOptions();

        using var stream = new FileStream(
            _jsonFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite
        );

        if (stream.Length == 0)
            return new TOptions();

        using var document = JsonDocument.Parse(stream, _serializerOptions is null ? default : new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        });
        if (!document.RootElement.TryGetProperty(_section, out JsonElement optionSection))
            return new TOptions();

        var value = optionSection.Deserialize<TOptions>(_serializerOptions);
        return value ?? new TOptions();
    }

    private sealed class ListenerRegistration(
        JsonFileOptionsMonitor<TOptions> parent,
        Action<TOptions, string?> listener
    ) : IDisposable
    {
        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
                return;

            parent.RemoveListener(listener);
            _isDisposed = true;
        }
    }
}
