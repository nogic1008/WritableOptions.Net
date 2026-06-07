using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Nogic.WritableOptions.Tests;

/// <summary>
/// Unit Test Class for <see cref="JsonWritableOptions{TOptions}"/>.
/// </summary>
public sealed class JsonWritableOptionsTest
{
    /// <summary>Used in <see cref="GenerateOption"/></summary>
    private static readonly Random _random = new();
    /// <summary>Generates <see cref="SampleOption"/> randomly for test.</summary>
    private static SampleOption GenerateOption() => new()
    {
        LastLaunchedAt = DateTime.Now,
        Interval = _random.Next(),
        ConnectionString = new Guid().ToString()
    };

    /// <summary>
    /// Normalize EndLine(LF, CR+LF) to <see cref="Environment.NewLine"/>.
    /// </summary>
    /// <param name="source">Source text</param>
    private static string NormalizeEndLine(string source)
        => source.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

    /// <summary>
    /// Constructor throws <see cref="ArgumentNullException"/> if param is <see langword="null"/>.
    /// </summary>
    /// <param name="jsonFilePath"><inheritdoc cref="JsonWritableOptions{TOptions}.JsonWritableOptions" path="/param[@name='jsonFilePath']" /></param>
    /// <param name="section"><inheritdoc cref="JsonWritableOptions{TOptions}.JsonWritableOptions" path="/param[@name='section']" /></param>
    /// <param name="options">set options as <see langword="null"/> or not</param>
    /// <param name="paramName">Expected <see cref="ArgumentNullException"/> param name</param>
    [Test]
    [DisplayName($"{nameof(JsonWritableOptions<>)}.ctor($jsonFilePath, $section, $options) throws {nameof(ArgumentNullException)}($paramName)")]
    [Arguments(null, "", "not null", "jsonFilePath")]
    [Arguments("", null, "not null", "section")]
    [Arguments("", "", null, "options")]
    public async Task Constructor_Throws_ArgumentNullException(string? jsonFilePath, string? section, string? options, string paramName)
    {
        // Arrange
        var optionsMonitor = options is null ? null : IOptionsMonitor<SampleOption>.Mock();

        // Act - Assert
        await Assert.That(() => new JsonWritableOptions<SampleOption>(jsonFilePath!, section!, optionsMonitor!))
            .Throws<ArgumentNullException>()
            .WithParameterName(paramName);
    }

    #region IOptionsMonitor<TOptions>
    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Value"/> returns TOptions via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [Test]
    [DisplayName($".{nameof(JsonWritableOptions<>.Value)} returns TOptions via {nameof(IOptionsMonitor<>)}<TOptions>")]
    public async Task Value_Returns_T()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = IOptionsMonitor<SampleOption>.Mock();
        _ = optionsMock.CurrentValue.Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock);

        // Act - Assert
        await Assert.That(sut.Value).EqualTo(sampleOption);
        await Assert.That(optionsMock.CurrentValue).WasCalled(Times.Once);
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.CurrentValue"/> returns TOptions via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [Test]
    [DisplayName($".{nameof(JsonWritableOptions<>.CurrentValue)} returns TOptions via {nameof(IOptionsMonitor<>)}<TOptions>")]
    public async Task CurrentValue_Returns_T()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = IOptionsMonitor<SampleOption>.Mock();
        _ = optionsMock.CurrentValue.Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock);

        // Act - Assert
        await Assert.That(sut.CurrentValue).EqualTo(sampleOption);
        await Assert.That(optionsMock.CurrentValue).WasCalled(Times.Once);
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Get"/> returns TOptions via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [Test]
    [DisplayName($".{nameof(JsonWritableOptions<>.Get)}(string?) returns TOptions via {nameof(IOptionsMonitor<>)}<TOptions>")]
    public async Task Get_Returns_T()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = IOptionsMonitor<SampleOption>.Mock();
        _ = optionsMock.Get(Any()).Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock);

        // Act - Assert
        await Assert.That(sut.Get("Foo")).EqualTo(sampleOption);
        await Assert.That(sut.Get("Bar")).EqualTo(sampleOption);
        await Assert.That(optionsMock.Get(Any())).WasCalled(Times.Exactly(2));
        await Assert.That(optionsMock.Get("Foo")).WasCalled(Times.Once);
        await Assert.That(optionsMock.Get("Bar")).WasCalled(Times.Once);
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.OnChange"/> returns <see cref="IDisposable"/> via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [Test]
    [DisplayName($".{nameof(JsonWritableOptions<>.OnChange)}(Action) returns {nameof(IDisposable)} via {nameof(IOptionsMonitor<>)}<TOptions>")]
    public async Task OnChange_Called_IOptionsMonitor_OnChange()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = IOptionsMonitor<SampleOption>.Mock();
        var action = static (SampleOption option, string? section)
            => Console.WriteLine($"{nameof(SampleOption)}:{section} changed to {option}");

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock);

        // Act
        _ = sut.OnChange(action);

        // Assert
        await Assert.That(optionsMock.OnChange(action)).WasCalled(Times.Once);
    }
    #endregion IOptionsMonitor<TOptions>

    private static readonly SampleOption _updatedOption = new()
    {
        LastLaunchedAt = new(2020, 12, 1),
        Interval = 5000,
        ConnectionString = "foo",
    };
    /// <summary>JSON serialized <see cref="_updatedOption"/></summary>
    // lang=json,strict
    private const string ExpectedJson = $$"""
    {
      "{{nameof(SampleOption)}}": {
        "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-12-01T00:00:00",
        "{{nameof(SampleOption.Interval)}}": 5000,
        "{{nameof(SampleOption.ConnectionString)}}": "foo"
      }
    }
    """;

    // lang=json,strict
    private const string EmptyJson = "{}";
    // lang=json,strict
    private const string ClassEmptyJson = $$"""
    {
      "{{nameof(SampleOption)}}": {}
    }
    """;
    // lang=json,strict
    private const string HasConfigJson = $$"""
    {
      "{{nameof(SampleOption)}}": {
          "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-10-01T00:00:00",
          "{{nameof(SampleOption.Interval)}}": 1000,
          "{{nameof(SampleOption.ConnectionString)}}": "bar"
      }
    }
    """;
    // lang=json,strict
    private const string HasCommentJson = $$"""
    {
      // Line Comment
      "{{nameof(SampleOption)}}": {
          /*
            Block Comment
           */
          "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-10-01T00:00:00",
          "{{nameof(SampleOption.Interval)}}": 1000,
          "{{nameof(SampleOption.ConnectionString)}}": "bar"
      }
    }
    """;
    // lang=json,strict
    private const string TrailingCommaJson = $$"""
    {
      "{{nameof(SampleOption)}}": {
          "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-10-01T00:00:00",
          "{{nameof(SampleOption.Interval)}}": 1000,
          "{{nameof(SampleOption.ConnectionString)}}": "bar",
      },
    }
    """;

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> writes expected JSON.
    /// </summary>
    /// <param name="fileText">Current JSON text</param>
    [Test]
    [DisplayName(".Update(TOptions) writes \"$fileText\" without BOM")]
    [Arguments(EmptyJson)]
    [Arguments(ClassEmptyJson)]
    [Arguments(HasConfigJson)]
    [Arguments(HasCommentJson)]
    [Arguments(TrailingCommaJson)]
    public async Task Update_Writes_Json(string fileText)
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(fileText);

        var optionsStub = IOptionsMonitor<SampleOption>.Mock();
        var configStub = IConfigurationRoot.Mock();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(_updatedOption);

        // Assert
        await Assert.That(tempFile.ReadAllText().ReplaceLineEndings("\n")).EqualTo(ExpectedJson);
        await Assert.That(configStub.Reload()).WasNeverCalled();
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> writes expected JSON with BOM.
    /// </summary>
    /// <param name="fileText">Current JSON text</param>
    [Test]
    [DisplayName(".Update(TOptions) writes \"$fileText\" with BOM")]
    [Arguments(EmptyJson)]
    [Arguments(ClassEmptyJson)]
    [Arguments(HasConfigJson)]
    [Arguments(HasCommentJson)]
    [Arguments(TrailingCommaJson)]
    public async Task Update_Writes_Json_WithBOM(string fileText)
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(fileText, Encoding.UTF8);

        var optionsStub = IOptionsMonitor<SampleOption>.Mock();
        var configStub = IConfigurationRoot.Mock();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(_updatedOption);

        // Assert
        await Assert.That(tempFile.ReadAllText(Encoding.UTF8).ReplaceLineEndings("\n")).EqualTo(ExpectedJson);
        await Assert.That(configStub.Reload()).WasNeverCalled();
    }

    private static readonly SampleOption _hasNullPropertiesOption = new()
    {
        LastLaunchedAt = new(2024, 03, 11),
        Interval = 2500,
        ConnectionString = null,
    };

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> writes <see langword="null"/> properties on default.
    /// </summary>
    [Test]
    [DisplayName(".Update(TOptions) writes null properties on default")]
    public async Task Update_Writes_Null_On_Default()
    {
        // Arrange
        using var tempFile = new TempFileProvider();

        var optionsStub = IOptionsMonitor<SampleOption>.Mock();
        var configStub = IConfigurationRoot.Mock();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(_hasNullPropertiesOption);

        // Assert
        await Assert.That(tempFile.ReadAllText()).Contains($"\"{nameof(SampleOption.ConnectionString)}\": null");
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> writes <see langword="null"/> properties when <see cref="JsonSerializerOptions.DefaultIgnoreCondition"/> is <see cref="JsonIgnoreCondition.WhenWritingNull"/>.
    /// </summary>
    [Test]
    [DisplayName(".Update(TOptions) does not write null properties when JsonSerializerOptions.DefaultIgnoreCondition is WhenWritingNull")]
    public async Task Update_DoesNot_Write_Null_When_Detected_On_SerializerOptions()
    {
        // Arrange
        using var tempFile = new TempFileProvider();

        var optionsStub = IOptionsMonitor<SampleOption>.Mock();
        var configStub = IConfigurationRoot.Mock();
        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, options, configStub.Object);

        // Act
        sut.Update(_hasNullPropertiesOption);

        // Assert
        await Assert.That(tempFile.ReadAllText()).DoesNotContain($"\"{nameof(SampleOption.ConnectionString)}\": null");
    }

    // lang=json,strict
    private const string OnlyHasOtherOptionJson = """
    {
      "fooOption": {}
    }
    """;
    // lang=json,strict
    private const string HasOtherOptionEmptyJson = $$"""
    {
      "fooOption": {},
      "{{nameof(SampleOption)}}": {}
    }
    """;
    // lang=json,strict
    private const string HasOtherOptionJson = $$"""
    {
      "fooOption": {},
      "{{nameof(SampleOption)}}": {
          "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-10-01T00:00:00",
          "{{nameof(SampleOption.Interval)}}": 1000,
          "{{nameof(SampleOption.ConnectionString)}}": "bar"
      }
    }
    """;

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> does not changes other JSON section.
    /// </summary>
    /// <param name="fileText">Current JSON text</param>
    [Test]
    [DisplayName(".Update(TOptions) does not changes other JSON section")]
    [Arguments(OnlyHasOtherOptionJson)]
    [Arguments(HasOtherOptionEmptyJson)]
    [Arguments(HasOtherOptionJson)]
    public async Task Update_DoesNot_Changes_Other_Section(string fileText)
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(fileText);

        var optionsStub = IOptionsMonitor<SampleOption>.Mock();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub);

        // Act
        sut.Update(_updatedOption, true);

        // Assert
        await Assert.That(tempFile.ReadAllText()).Contains("\"fooOption\": {},");
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> calls <see cref="IConfigurationRoot.Reload"/>.
    /// </summary>
    [Test]
    [DisplayName(".Update(TOptions, true) calls IConfigurationRoot.Reload()")]
    public async Task Update_Calls_Reload()
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(EmptyJson);

        var optionsStub = IOptionsMonitor<SampleOption>.Mock();
        var configStub = IConfigurationRoot.Mock();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(_updatedOption, true);

        // Assert
        await Assert.That(tempFile.ReadAllText().ReplaceLineEndings("\n")).EqualTo(ExpectedJson);
        await Assert.That(configStub.Reload()).WasCalled(Times.Once);
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> creates new JSON file if not exists.
    /// </summary>
    [Test]
    [DisplayName(".Update(TOptions) creates new JSON file if not exists")]
    public async ValueTask Update_Creates_File()
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        File.Delete(tempFile.Path);

        var optionsStub = IOptionsMonitor<SampleOption>.Mock();
        var configStub = IConfigurationRoot.Mock();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(_updatedOption);

        // Assert
        await Assert.That(new FileInfo(tempFile.Path)).Exists();
        await Assert.That(tempFile.ReadAllText().ReplaceLineEndings("\n")).EqualTo(ExpectedJson);
    }

    /// <summary>Provides temporary file for testing.</summary>
    private class TempFileProvider : IDisposable
    {
        /// <summary>Temporary file path</summary>
        public string Path { get; } = System.IO.Path.GetTempFileName();

        /// <summary>Call <see cref="Dispose(bool)"/> or not</summary>
        /// <remarks>Compliant Dispose pattern.</remarks>
        private bool _disposedValue;

        /// <inheritdoc cref="IDisposable.Dispose" />
        /// <param name="disposing">Call from <see cref="Dispose"/> or not.</param>
        /// <remarks>Compliant Dispose pattern.</remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                // Managed resource
            }

            // Unmanaged resource
            if (File.Exists(Path))
                File.Delete(Path);

            _disposedValue = true;
        }

        /// <inheritdoc/>
        /// <remarks>Compliant Dispose pattern.</remarks>
        ~TempFileProvider() => Dispose(false);

        /// <inheritdoc/>
        /// <remarks>Compliant Dispose pattern.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Call <see cref="File.ReadAllText(string)"/>.</summary>
        public string ReadAllText() => File.ReadAllText(Path);

        /// <summary>Call <see cref="File.ReadAllText(string, Encoding)"/>.</summary>
        /// <inheritdoc cref="File.ReadAllText(string, Encoding)" path="/param[@name='encoding']" />
        public string ReadAllText(Encoding encoding) => File.ReadAllText(Path, encoding);

        /// <summary>Call <see cref="File.AppendAllText(string, string)"/>.</summary>
        /// <inheritdoc cref="File.AppendAllText(string, string)" path="/param[@name='contents']" />
        public void AppendAllText(string contents) => File.AppendAllText(Path, contents);

        /// <summary>Call <see cref="File.AppendAllText(string, string, Encoding)"/>.</summary>
        /// <inheritdoc cref="File.AppendAllText(string, string, Encoding)" path="/param[@name='contents']" />
        /// <inheritdoc cref="File.AppendAllText(string, string, Encoding)" path="/param[@name='encoding']" />
        public void AppendAllText(string contents, Encoding encoding) => File.AppendAllText(Path, contents, encoding);
    }
}
