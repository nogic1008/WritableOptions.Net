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
    /// <inheritdoc cref="JsonWritableOptions(string, string, IOptionsMonitor{TOptions}, IConfigurationRoot?)" path="/param[@name='jsonFilePath']"/>
    /// </summary>
    private readonly string _jsonFilePath;
    /// <summary>
    /// <inheritdoc cref="JsonWritableOptions(string, string, IOptionsMonitor{TOptions}, IConfigurationRoot?)" path="/param[@name='section']"/>
    /// </summary>
    private readonly JsonEncodedText _section;
    /// <summary>
    /// <inheritdoc cref="JsonWritableOptions(string, string, IOptionsMonitor{TOptions}, IConfigurationRoot?)" path="/param[@name='options']"/>
    /// </summary>
    private readonly IOptionsMonitor<TOptions> _options;
    /// <summary>
    /// <inheritdoc cref="JsonWritableOptions(string, string, IOptionsMonitor{TOptions}, IConfigurationRoot?)" path="/param[@name='configuration']"/>
    /// </summary>
    private readonly IConfigurationRoot? _configuration;

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
    public JsonWritableOptions(string jsonFilePath, string section, IOptionsMonitor<TOptions> options, IConfigurationRoot? configuration = null)
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
    public TOptions Get(string name) => _options.Get(name);

    /// <inheritdoc/>
    public IDisposable OnChange(Action<TOptions, string> listener) => _options.OnChange(listener);

    /// <inheritdoc/>
    public void Update(TOptions changedValue, bool reload = false)
    {
        ReadOnlyMemory<byte> jsonByteData = File.ReadAllBytes(_jsonFilePath);

        // Check BOM
        bool isBOM = false;
        ReadOnlySpan<byte> utf8bom = Encoding.UTF8.GetPreamble();
        if (jsonByteData.Span.StartsWith(utf8bom))
        {
            isBOM = true;
#if NETCOREAPP3_1_OR_GREATER
            jsonByteData = jsonByteData[utf8bom.Length..];
#else
            jsonByteData = jsonByteData.Slice(utf8bom.Length);
#endif
        }

        using var jsonDocument = JsonDocument.Parse(jsonByteData);

        using (var stream = File.OpenWrite(_jsonFilePath))
        {
            // Write BOM
            if (isBOM)
            {
#if NETCOREAPP3_1_OR_GREATER
                stream.Write(utf8bom);
#else
                stream.Write(utf8bom.ToArray(), 0, utf8bom.Length);
#endif
            }

            var writer = new Utf8JsonWriter(stream, new()
            {
                Indented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            writer.WriteStartObject();
            bool isWritten = false;
            var optionsElement = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(changedValue));
            foreach (var element in jsonDocument.RootElement.EnumerateObject())
            {
                if (!element.NameEquals(_section.EncodedUtf8Bytes))
                {
                    element.WriteTo(writer);
                    continue;
                }
                writer.WritePropertyName(_section);
                optionsElement.WriteTo(writer);
                isWritten = true;
            }
            if (!isWritten)
            {
                writer.WritePropertyName(_section);
                optionsElement.WriteTo(writer);
            }
            writer.WriteEndObject();
            writer.Flush();
            stream.SetLength(stream.Position);
        }
        if (reload)
            _configuration?.Reload();
    }
}
