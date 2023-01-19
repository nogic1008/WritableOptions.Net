using System.Buffers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Nogic.WritableOptions;

/// <summary>
/// Read and write <typeparamref name="TOptions"/> in JSON file.
/// </summary>
/// <typeparam name="TOptions">Options type.</typeparam>
public class JsonWritableOptions<TOptions> : IWritableOptions<TOptions> where TOptions : class, new()
{
    /// <summary>
    /// <inheritdoc cref="JsonWritableOptions(string, string, IOptionsMonitor{TOptions}, IConfiguration?)" path="/param[@name='jsonFilePath']"/>
    /// </summary>
    private readonly string _jsonFilePath;
    /// <summary>
    /// <inheritdoc cref="JsonWritableOptions(string, string, IOptionsMonitor{TOptions}, IConfiguration?)" path="/param[@name='section']"/>
    /// </summary>
    private readonly JsonEncodedText _section;
    /// <summary>
    /// <inheritdoc cref="JsonWritableOptions(string, string, IOptionsMonitor{TOptions}, IConfiguration?)" path="/param[@name='options']"/>
    /// </summary>
    private readonly IOptionsMonitor<TOptions> _options;
    /// <summary>
    /// <inheritdoc cref="JsonWritableOptions(string, string, IOptionsMonitor{TOptions}, IConfiguration?)" path="/param[@name='configuration']"/>
    /// </summary>
    private readonly IConfiguration? _configuration;

    private static readonly JsonWriterOptions _jsonWriterOptions = new()
    {
        Indented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Initializes a new instance of the JsonWritableOptions class.
    /// </summary>
    /// <param name="jsonFilePath">JSON file path to read/write settings.</param>
    /// <param name="section">JSON property name to store settings.</param>
    /// <param name="options">Instance to read <typeparamref name="TOptions"/>. Should be referenced <paramref name="jsonFilePath"/>.</param>
    /// <param name="configuration">Configuration root for reload</param>
    /// <exception cref="ArgumentNullException">
    /// Throws if <paramref name="jsonFilePath"/>, <paramref name="section"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public JsonWritableOptions(string jsonFilePath, string section, IOptionsMonitor<TOptions> options, IConfiguration? configuration = null)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(jsonFilePath);
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(options);
#else
        ThrowIfNull(jsonFilePath, nameof(jsonFilePath));
        ThrowIfNull(section, nameof(section));
        ThrowIfNull(options, nameof(options));
#endif

        _jsonFilePath = jsonFilePath;
        _section = JsonEncodedText.Encode(section);
        _options = options;
        _configuration = configuration;
#if !NET6_0_OR_GREATER
        // Port of ArgumentNullException.ThrowIfNull
        static void ThrowIfNull(object? argument, string paramName)
        {
            if (argument is null)
                throw new ArgumentNullException(paramName);
        }
#endif
    }

    /// <inheritdoc/>
    public TOptions Value => _options.CurrentValue;

    /// <inheritdoc/>
    public TOptions CurrentValue => _options.CurrentValue;

    /// <inheritdoc/>
    public TOptions Get(string? name) => _options.Get(name);

    /// <inheritdoc/>
    public IDisposable? OnChange(Action<TOptions, string?> listener) => _options.OnChange(listener);

    /// <inheritdoc/>
    public void Update(TOptions changedValue, bool reload = false)
    {
        using (var stream = new FileStream(_jsonFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent((int)stream.Length);
            try
            {
#if NETCOREAPP2_1_OR_GREATER
                _ = stream.Read(buffer);
#else
                _ = stream.Read(buffer, 0, buffer.Length);
#endif
                ReadOnlySpan<byte> utf8Json = buffer.AsSpan();

                // Check BOM
#if NETCOREAPP2_1_OR_GREATER
                var utf8bom = Encoding.UTF8.Preamble;
#else
                ReadOnlySpan<byte> utf8bom = Encoding.UTF8.GetPreamble();
#endif
                if (utf8Json.StartsWith(utf8bom))
                {
#pragma warning disable IDE0057
                    utf8Json = utf8Json.Slice(utf8bom.Length);
#pragma warning restore IDE0057
                    _ = stream.Seek(utf8bom.Length, SeekOrigin.Begin);
                }
                else
                {
                    stream.Position = 0;
                }

                var reader = new Utf8JsonReader(utf8Json.Length > 0 ? utf8Json : "{}"u8);
                var currentJson = JsonElement.ParseValue(ref reader);
                var writer = new Utf8JsonWriter(stream, _jsonWriterOptions);

                writer.WriteStartObject(); // {
                bool isWritten = false;
                var serializedOptionsValue = JsonSerializer.SerializeToElement(changedValue);
                foreach (var element in currentJson.EnumerateObject())
                {
                    if (!element.NameEquals(_section.EncodedUtf8Bytes))
                    {
                        element.WriteTo(writer);
                        continue;
                    }
                    writer.WritePropertyName(_section);
                    serializedOptionsValue.WriteTo(writer);
                    isWritten = true;
                }
                if (!isWritten)
                {
                    writer.WritePropertyName(_section);
                    serializedOptionsValue.WriteTo(writer);
                }
                writer.WriteEndObject(); // }

                writer.Flush();
                stream.SetLength(stream.Position);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        if (reload && _configuration is IConfigurationRoot configurationRoot)
            configurationRoot.Reload();
    }
}
