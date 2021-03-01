using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Nogic.WritableOptions
{
    public class JsonWritableOptions<TOptions> : IWritableOptions<TOptions> where TOptions : class, new()
    {
        private readonly IOptionsMonitor<TOptions> _options;
        private readonly string _jsonFilePath;
        private readonly string _section;
        private readonly IConfigurationRoot? _configuration;

        public JsonWritableOptions(string jsonFilePath, string section, IOptionsMonitor<TOptions> options, IConfigurationRoot? configuration)
            => (_jsonFilePath, _options, _section, _configuration) = (jsonFilePath, options, section, configuration);

        public TOptions Value => _options.CurrentValue;

        public TOptions Get(string name) => _options.Get(name);

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
