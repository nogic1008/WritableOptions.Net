using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace Nogic.WritableOptions.Tests;

/// <summary>
/// Unit Test Class for <see cref="JsonWritableOptions{TOptions}"/>.
/// </summary>
[TestClass]
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
    /// <param name="hasOptions">set options as <see langword="null"/> or not</param>
    /// <param name="paramName">Expected <see cref="ArgumentNullException"/> param name</param>
    [TestMethod($"Constructor throws {nameof(ArgumentNullException)}")]
    [DataRow(null, "", true, "jsonFilePath", DisplayName = $"{nameof(JsonWritableOptions<SampleOption>)}(null, \"\", new()) throws {nameof(ArgumentNullException)}(jsonFilePath)")]
    [DataRow("", null, true, "section", DisplayName = $"{nameof(JsonWritableOptions<SampleOption>)}(\"\", null, new()) throws {nameof(ArgumentNullException)}(section)")]
    [DataRow("", "", false, "options", DisplayName = $"{nameof(JsonWritableOptions<SampleOption>)}(\"\", \"\", null) throws {nameof(ArgumentNullException)}(options)")]
    public void Constructor_Throws_ArgumentNullException(string? jsonFilePath, string? section, bool hasOptions, string paramName)
    {
        // Arrange
        var options = hasOptions ? new Mock<IOptionsMonitor<SampleOption>>().Object : null;
        var constructor = () => new JsonWritableOptions<SampleOption>(jsonFilePath!, section!, options!);

        // Act - Assert
        _ = constructor.Should().ThrowExactly<ArgumentNullException>().WithParameterName(paramName);
    }

    #region IOptionsMonitor<TOptions>
    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Value"/> returns TOptions via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [TestMethod($".{nameof(JsonWritableOptions<SampleOption>.Value)} returns TOptions via {nameof(IOptionsMonitor<SampleOption>)}<TOptions>")]
    public void Value_Returns_T()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = new Mock<IOptionsMonitor<SampleOption>>();
        _ = optionsMock.SetupGet(static m => m.CurrentValue).Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock.Object);

        // Act - Assert
        _ = sut.Value.Should().Be(sampleOption);
        optionsMock.VerifyGet(static m => m.CurrentValue, Times.Once());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.CurrentValue"/> returns TOptions via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [TestMethod($".{nameof(JsonWritableOptions<SampleOption>.CurrentValue)} returns TOptions via {nameof(IOptionsMonitor<SampleOption>)}<TOptions>")]
    public void CurrentValue_Returns_T()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = new Mock<IOptionsMonitor<SampleOption>>();
        _ = optionsMock.SetupGet(static m => m.CurrentValue).Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock.Object);

        // Act - Assert
        _ = sut.CurrentValue.Should().Be(sampleOption);
        optionsMock.VerifyGet(static m => m.CurrentValue, Times.Once());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Get"/> returns TOptions via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [TestMethod($".{nameof(JsonWritableOptions<SampleOption>.Get)}(string?) returns TOptions via {nameof(IOptionsMonitor<SampleOption>)}<TOptions>")]
    public void Get_Returns_T()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = new Mock<IOptionsMonitor<SampleOption>>();
        _ = optionsMock.Setup(static m => m.Get(It.IsAny<string>())).Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock.Object);

        // Act - Assert
        _ = sut.Get("Foo").Should().Be(sampleOption);
        _ = sut.Get("Bar").Should().Be(sampleOption);

        optionsMock.Verify(static m => m.Get(It.IsAny<string>()), Times.Exactly(2));
        optionsMock.Verify(static m => m.Get("Foo"), Times.Once());
        optionsMock.Verify(static m => m.Get("Bar"), Times.Once());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.OnChange"/> returns <see cref="IDisposable"/> via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [TestMethod($".{nameof(JsonWritableOptions<SampleOption>.OnChange)}(Action) returns {nameof(IDisposable)} via {nameof(IOptionsMonitor<SampleOption>)}<TOptions>")]
    public void OnChange_Called_IOptionsMonitor_OnChange()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = new Mock<IOptionsMonitor<SampleOption>>();
        var action = static (SampleOption option, string? section)
            => Console.WriteLine($"{nameof(SampleOption)}:{section} changed to {option}");

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock.Object);

        // Act
        _ = sut.OnChange(action);

        // Assert
        optionsMock.Verify(m => m.OnChange(action), Times.Once());
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
    [TestMethod(".Update(TOptions) writes expected JSON")]
    [DataRow(EmptyJson, DisplayName = ".Update(TOptions) writes expected JSON")]
    [DataRow(ClassEmptyJson, DisplayName = ".Update(TOptions) writes expected JSON")]
    [DataRow(HasConfigJson, DisplayName = ".Update(TOptions) writes expected JSON")]
    [DataRow(HasCommentJson, DisplayName = ".Update(TOptions) writes expected JSON")]
    [DataRow(TrailingCommaJson, DisplayName = ".Update(TOptions) writes expected JSON")]
    public void Update_Writes_Json(string fileText)
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(fileText);

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;
        var configStub = new Mock<IConfigurationRoot>();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(_updatedOption);

        // Assert
        _ = tempFile.ReadAllText().Should().Be(NormalizeEndLine(ExpectedJson));
        configStub.Verify(static m => m.Reload(), Times.Never());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> writes expected JSON with BOM.
    /// </summary>
    /// <param name="fileText">Current JSON text</param>
    [TestMethod(".Update(TOptions) writes expected JSON with BOM")]
    [DataRow(EmptyJson, DisplayName = ".Update(TOptions) writes expected JSON with BOM")]
    [DataRow(ClassEmptyJson, DisplayName = ".Update(TOptions) writes expected JSON with BOM")]
    [DataRow(HasConfigJson, DisplayName = ".Update(TOptions) writes expected JSON with BOM")]
    [DataRow(HasCommentJson, DisplayName = ".Update(TOptions) writes expected JSON with BOM")]
    [DataRow(TrailingCommaJson, DisplayName = ".Update(TOptions) writes expected JSON")]
    public void Update_Writes_Json_WithBOM(string fileText)
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(fileText, Encoding.UTF8);

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;
        var configStub = new Mock<IConfigurationRoot>();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(_updatedOption);

        // Assert
        _ = tempFile.ReadAllText(Encoding.UTF8).Should().Be(NormalizeEndLine(ExpectedJson));
        configStub.Verify(static m => m.Reload(), Times.Never());
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
    [TestMethod(".Update(TOptions) writes null properties on default")]
    public void Update_Writes_Null_On_Default()
    {
        // Arrange
        using var tempFile = new TempFileProvider();

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;
        var configStub = new Mock<IConfigurationRoot>();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(_hasNullPropertiesOption);

        // Assert
        tempFile.ReadAllText().Should().Contain($"\"{nameof(SampleOption.ConnectionString)}\": null");
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> writes <see langword="null"/> properties when <see cref="JsonSerializerOptions.DefaultIgnoreCondition"/> is <see cref="JsonIgnoreCondition.WhenWritingNull"/>.
    /// </summary>
    [TestMethod(".Update(TOptions) does not write null properties when JsonSerializerOptions.DefaultIgnoreCondition is WhenWritingNull")]
    public void Update_DoesNot_Write_Null_When_Detected_On_SerializerOptions()
    {
        // Arrange
        using var tempFile = new TempFileProvider();

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;
        var configStub = new Mock<IConfigurationRoot>();
        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, options, configStub.Object);

        // Act
        sut.Update(_hasNullPropertiesOption);

        // Assert
        tempFile.ReadAllText().Should().NotContain($"\"{nameof(SampleOption.ConnectionString)}\": null");
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
    [TestMethod(".Update(TOptions) does not changes other JSON section")]
    [DataRow(OnlyHasOtherOptionJson, DisplayName = ".Update(TOptions) does not changes other JSON section")]
    [DataRow(HasOtherOptionEmptyJson, DisplayName = ".Update(TOptions) does not changes other JSON section")]
    [DataRow(HasOtherOptionJson, DisplayName = ".Update(TOptions) does not changes other JSON section")]
    public void Update_DoesNot_Changes_Other_Section(string fileText)
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(fileText);

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub);

        // Act
        sut.Update(_updatedOption, true);

        // Assert
        _ = tempFile.ReadAllText().Should().Contain("\"fooOption\": {},");
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> calls <see cref="IConfigurationRoot.Reload"/>.
    /// </summary>
    [TestMethod(".Update(TOptions, true) calls IConfigurationRoot.Reload()")]
    public void Update_Calls_Reload()
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(EmptyJson);

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;
        var configStub = new Mock<IConfigurationRoot>();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(_updatedOption, true);

        // Assert
        _ = tempFile.ReadAllText().Should().Be(NormalizeEndLine(ExpectedJson));
        configStub.Verify(static m => m.Reload(), Times.Once());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> creates new JSON file if not exists.
    /// </summary>
    [TestMethod(".Update(TOptions) creates new JSON file if not exists")]
    public void Update_Creates_File()
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        File.Delete(tempFile.Path);

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;
        var configStub = new Mock<IConfigurationRoot>().Object;

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub);

        // Act
        sut.Update(_updatedOption);

        // Assert
        _ = File.Exists(tempFile.Path).Should().BeTrue();
        _ = tempFile.ReadAllText().Should().Be(NormalizeEndLine(ExpectedJson));
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
