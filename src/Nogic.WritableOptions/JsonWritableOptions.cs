using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Nogic.WritableOptions
{
    /// <summary>
    /// Read and write <typeparamref name="TOptions"/> in JSON file.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    public class JsonWritableOptions<TOptions> : IWritableOptions<TOptions> where TOptions : class, new()
    {
        private readonly IOptionsMonitor<TOptions> _options;
        private readonly string _jsonFilePath;
        private readonly string _section;
        private readonly IConfigurationRoot? _configuration;

        /// <summary>
        /// Initializes a new instance of the JsonWritableOptions class.
        /// </summary>
        /// <param name="jsonFilePath">JSON file path to read/write settings.</param>
        /// <param name="section">JSON property name to store settings.</param>
        /// <param name="options">Instance to read <typeparamref name="TOptions"/>. Should be referenced <paramref name="jsonFilePath"/>.</param>
        /// <param name="configuration">Configuration root for reload</param>
        public JsonWritableOptions(string jsonFilePath, string section, IOptionsMonitor<TOptions> options, IConfigurationRoot? configuration = null)
            => (_jsonFilePath, _section, _options, _configuration) = (jsonFilePath, section, options, configuration);

        /// <inheritdoc/>
        public TOptions Value => _options.CurrentValue;

        /// <inheritdoc/>
        public TOptions Get(string name) => _options.Get(name);

        /// <inheritdoc/>
        public void Update(TOptions changedValue, bool reload = false)
        {
            using var jsonDocument = JsonDocument.Parse(File.ReadAllBytes(_jsonFilePath));

            using var stream = File.OpenWrite(_jsonFilePath);
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
                if (element.Name != _section)
                {
                    element.WriteTo(writer);
                    continue;
                }
                writer.WritePropertyName(element.Name);
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

            if (reload)
                _configuration?.Reload();
        }
    }
}
