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
    /// <summary>
    /// Constructor throws <see cref="ArgumentNullException"/> if param is <see langword="null"/>.
    /// </summary>
    /// <param name="jsonFilePath"><inheritdoc cref="JsonWritableOptions{TOptions}.JsonWritableOptions" path="/param[@name='jsonFilePath']" /></param>
    /// <param name="section"><inheritdoc cref="JsonWritableOptions{TOptions}.JsonWritableOptions" path="/param[@name='section']" /></param>
    /// <param name="paramName">Expected <see cref="ArgumentNullException"/> param name</param>
    [Test]
    [DisplayName(
        $"{nameof(JsonWritableOptions<>)}.ctor($jsonFilePath, $section) throws {nameof(ArgumentNullException)}($paramName)"
    )]
    [Arguments(null, "", "jsonFilePath")]
    [Arguments("", null, "section")]
    public async Task Constructor_Throws_ArgumentNullException(
        string? jsonFilePath,
        string? section,
        string paramName
    ) =>
        // Act - Assert
        await Assert
            .That(() => new JsonWritableOptions<SampleOption>(jsonFilePath!, section!))
            .Throws<ArgumentNullException>()
            .WithParameterName(paramName);

    #region Internal Options Monitor
    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Value"/> returns TOptions from target JSON file section.
    /// </summary>
    [Test]
    [DisplayName($".{nameof(JsonWritableOptions<>.Value)} returns TOptions from JSON section")]
    public async Task Value_Returns_T()
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(HasConfigJson);

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption));

        // Act - Assert
        await Assert.That(sut.Value.LastLaunchedAt).EqualTo(new DateTime(2020, 10, 1));
        await Assert.That(sut.Value.Interval).EqualTo(1000);
        await Assert.That(sut.Value.ConnectionString).EqualTo("bar");
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.CurrentValue"/> returns TOptions from target JSON file section.
    /// </summary>
    [Test]
    [DisplayName(
        $".{nameof(JsonWritableOptions<>.CurrentValue)} returns TOptions from JSON section"
    )]
    public async Task CurrentValue_Returns_T()
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(HasConfigJson);

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption));

        // Act - Assert
        await Assert.That(sut.CurrentValue.LastLaunchedAt).EqualTo(new DateTime(2020, 10, 1));
        await Assert.That(sut.CurrentValue.Interval).EqualTo(1000);
        await Assert.That(sut.CurrentValue.ConnectionString).EqualTo("bar");
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Get"/> returns TOptions from target JSON file section.
    /// </summary>
    [Test]
    [DisplayName(
        $".{nameof(JsonWritableOptions<>.Get)}(string?) returns TOptions from JSON section"
    )]
    public async Task Get_Returns_T()
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(HasConfigJson);

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption));

        // Act - Assert
        await Assert.That(sut.Get("Foo").Interval).EqualTo(1000);
        await Assert.That(sut.Get("Bar").ConnectionString).EqualTo("bar");
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.OnChange"/> notifies listeners after <see cref="JsonWritableOptions{TOptions}.Update"/>.
    /// </summary>
    [Test]
    [DisplayName($".{nameof(JsonWritableOptions<>.OnChange)}(Action) is called after Update")]
    public async Task OnChange_Called_After_Update()
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(EmptyJson);
        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption));
        SampleOption? actual = null;
        string? actualName = null;

        _ = sut.OnChange(
            (option, name) =>
            {
                actual = option;
                actualName = name;
            }
        );

        // Act
        sut.Update(_updatedOption);

        // Assert
        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.Interval).EqualTo(_updatedOption.Interval);
        await Assert.That(actualName).EqualTo(Options.DefaultName);
    }
    #endregion Internal Options Monitor

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

        var configStub = IConfigurationRoot.Mock();

        var sut = new JsonWritableOptions<SampleOption>(
            tempFile.Path,
            nameof(SampleOption),
            configStub.Object
        );

        // Act
        sut.Update(_updatedOption);

        // Assert
        await Assert
            .That(tempFile.ReadAllText().ReplaceLineEndings("\n"))
            .EqualTo(ExpectedJson.ReplaceLineEndings("\n"));
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

        var configStub = IConfigurationRoot.Mock();

        var sut = new JsonWritableOptions<SampleOption>(
            tempFile.Path,
            nameof(SampleOption),
            configStub.Object
        );

        // Act
        sut.Update(_updatedOption);

        // Assert
        await Assert
            .That(tempFile.ReadAllText(Encoding.UTF8).ReplaceLineEndings("\n"))
            .EqualTo(ExpectedJson.ReplaceLineEndings("\n"));
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

        var configStub = IConfigurationRoot.Mock();

        var sut = new JsonWritableOptions<SampleOption>(
            tempFile.Path,
            nameof(SampleOption),
            configStub.Object
        );

        // Act
        sut.Update(_hasNullPropertiesOption);

        // Assert
        await Assert
            .That(tempFile.ReadAllText())
            .Contains($"\"{nameof(SampleOption.ConnectionString)}\": null");
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> writes <see langword="null"/> properties when <see cref="JsonSerializerOptions.DefaultIgnoreCondition"/> is <see cref="JsonIgnoreCondition.WhenWritingNull"/>.
    /// </summary>
    [Test]
    [DisplayName(
        ".Update(TOptions) does not write null properties when JsonSerializerOptions.DefaultIgnoreCondition is WhenWritingNull"
    )]
    public async Task Update_DoesNot_Write_Null_When_Detected_On_SerializerOptions()
    {
        // Arrange
        using var tempFile = new TempFileProvider();

        var configStub = IConfigurationRoot.Mock();
        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        var sut = new JsonWritableOptions<SampleOption>(
            tempFile.Path,
            nameof(SampleOption),
            options,
            configStub.Object
        );

        // Act
        sut.Update(_hasNullPropertiesOption);

        // Assert
        await Assert
            .That(tempFile.ReadAllText())
            .DoesNotContain($"\"{nameof(SampleOption.ConnectionString)}\": null");
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

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption));

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

        var configStub = IConfigurationRoot.Mock();

        var sut = new JsonWritableOptions<SampleOption>(
            tempFile.Path,
            nameof(SampleOption),
            configStub.Object
        );

        // Act
        sut.Update(_updatedOption, true);

        // Assert
        await Assert
            .That(tempFile.ReadAllText().ReplaceLineEndings("\n"))
            .EqualTo(ExpectedJson.ReplaceLineEndings("\n"));
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

        var configStub = IConfigurationRoot.Mock();

        var sut = new JsonWritableOptions<SampleOption>(
            tempFile.Path,
            nameof(SampleOption),
            configStub.Object
        );

        // Act
        sut.Update(_updatedOption);

        // Assert
        await Assert.That(new FileInfo(tempFile.Path)).Exists();
        await Assert
            .That(tempFile.ReadAllText().ReplaceLineEndings("\n"))
            .EqualTo(ExpectedJson.ReplaceLineEndings("\n"));
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
        public void AppendAllText(string contents, Encoding encoding) =>
            File.AppendAllText(Path, contents, encoding);
    }
}
