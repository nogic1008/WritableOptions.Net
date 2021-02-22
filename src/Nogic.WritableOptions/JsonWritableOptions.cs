using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Nogic.WritableOptions
{
    public class JsonWritableOptions<TOptions> : IWritableOptions<TOptions> where TOptions : class, new()
    {
        private readonly IHostEnvironment _environment;
        private readonly IOptionsMonitor<TOptions> _options;
        private readonly string _section;
        private readonly string _file;

        public JsonWritableOptions(IHostEnvironment environment, IOptionsMonitor<TOptions> options, string section, string file)
            => (_environment, _options, _section, _file) = (environment, options, section, file);

        public TOptions Value => _options.CurrentValue;

        public TOptions Get(string name) => _options.Get(name);

        public void Update(TOptions changedValue)
        {
            var fileProvider = _environment.ContentRootFileProvider;
            var fileInfo = fileProvider.GetFileInfo(_file);
            string physicalPath = fileInfo.PhysicalPath;

            using var jsonDocument = JsonDocument.Parse(File.ReadAllBytes(physicalPath));

            using var stream = File.OpenWrite(physicalPath);
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
        }
    }
}
