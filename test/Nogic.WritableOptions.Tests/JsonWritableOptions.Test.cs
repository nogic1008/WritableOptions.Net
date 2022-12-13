using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;

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
    /// <param name="hasOptions">set options as <see langword="null"/> or not</param>
    /// <param name="paramName">Expected <see cref="ArgumentNullException"/> param name</param>
    [Theory(DisplayName = $"Constructor throws {nameof(ArgumentNullException)}")]
    [InlineData(null, "", true, "jsonFilePath")]
    [InlineData("", null, true, "section")]
    [InlineData("", "", false, "options")]
    public void Constructor_Throws_ArgumentNullException(string? jsonFilePath, string? section, bool hasOptions, string paramName)
    {
        // Arrange
        var options = hasOptions ? new Mock<IOptionsMonitor<SampleOption>>().Object : null;
        var constructor = () => new JsonWritableOptions<SampleOption>(jsonFilePath!, section!, options!);

        // Act - Assert
        _ = constructor.Should().ThrowExactly<ArgumentNullException>().WithParameterName(paramName);
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Value"/> returns TOptions via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [Fact(DisplayName = ".Value returns TOptions via IOptionsMonitor<TOptions>")]
    public void Value_Returns_T()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = new Mock<IOptionsMonitor<SampleOption>>();
        _ = optionsMock.SetupGet(m => m.CurrentValue).Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock.Object);

        // Act - Assert
        _ = sut.Value.Should().Be(sampleOption);
        optionsMock.VerifyGet(m => m.CurrentValue, Times.Once());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.CurrentValue"/> returns TOptions via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [Fact(DisplayName = ".CurrentValue returns TOptions via IOptionsMonitor<TOptions>")]
    public void CurrentValue_Returns_T()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = new Mock<IOptionsMonitor<SampleOption>>();
        _ = optionsMock.SetupGet(m => m.CurrentValue).Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock.Object);

        // Act - Assert
        _ = sut.CurrentValue.Should().Be(sampleOption);
        optionsMock.VerifyGet(m => m.CurrentValue, Times.Once());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Get"/> returns TOptions via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [Fact(DisplayName = ".Get(string) returns TOptions via IOptionsMonitor<TOptions>")]
    public void Get_Returns_T()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = new Mock<IOptionsMonitor<SampleOption>>();
        _ = optionsMock.Setup(m => m.Get(It.IsAny<string>())).Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock.Object);

        // Act - Assert
        _ = sut.Get("Foo").Should().Be(sampleOption);
        _ = sut.Get("Bar").Should().Be(sampleOption);

        optionsMock.Verify(m => m.Get(It.IsAny<string>()), Times.Exactly(2));
        optionsMock.Verify(m => m.Get("Foo"), Times.Once());
        optionsMock.Verify(m => m.Get("Bar"), Times.Once());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.OnChange"/> returns <see cref="IDisposable"/> via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [Fact(DisplayName = $".OnChange(Action) returns {nameof(IDisposable)} via IOptionsMonitor<TOptions>")]
    public void OnChange_Called_IOptionsMonitor_OnChange()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = new Mock<IOptionsMonitor<SampleOption>>();
        var action = (SampleOption option, string? section)
            => Console.WriteLine($"{nameof(SampleOption)}:{section} changed to {option}");

        var sut = new JsonWritableOptions<SampleOption>("", "", optionsMock.Object);

        // Act
        _ = sut.OnChange(action);

        // Assert
        optionsMock.Verify(m => m.OnChange(action), Times.Once());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> writes expected JSON.
    /// </summary>
    /// <param name="fileText">Current JSON text</param>
    [Theory(DisplayName = ".Update(TOptions) writes expected JSON")]
    [InlineData(/*lang=json,strict*/ "{}")]
    [InlineData(/*lang=json,strict*/ $$"""
    {
      "{{nameof(SampleOption)}}": {}
    }
    """)]
    [InlineData(/*lang=json,strict*/ $$"""
    {
      "{{nameof(SampleOption)}}": {
          "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-10-01T00:00:00",
          "{{nameof(SampleOption.Interval)}}": 1000,
          "{{nameof(SampleOption.ConnectionString)}}": "bar"
      }
    }
    """)]
    public void Update_Writes_Json(string fileText)
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(fileText);

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;
        var configStub = new Mock<IConfigurationRoot>();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(new()
        {
            LastLaunchedAt = new(2020, 12, 1),
            Interval = 5000,
            ConnectionString = "foo",
        });

        // Assert
        /*lang=json,strict*/
        const string expectedJson =
        $$"""
        {
          "{{nameof(SampleOption)}}": {
            "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-12-01T00:00:00",
            "{{nameof(SampleOption.Interval)}}": 5000,
            "{{nameof(SampleOption.ConnectionString)}}": "foo"
          }
        }
        """;
        _ = tempFile.ReadAllText().Should().Be(NormalizeEndLine(expectedJson));
        configStub.Verify(m => m.Reload(), Times.Never());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> writes expected JSON with BOM.
    /// </summary>
    /// <param name="fileText">Current JSON text</param>
    [Theory(DisplayName = ".Update(TOptions) writes expected JSON with BOM")]
    [InlineData(/*lang=json,strict*/ "{}")]
    [InlineData(/*lang=json,strict*/ $$"""
    {
      "{{nameof(SampleOption)}}": {}
    }
    """)]
    [InlineData(/*lang=json,strict*/ $$"""
    {
      "{{nameof(SampleOption)}}": {
          "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-10-01T00:00:00",
          "{{nameof(SampleOption.Interval)}}": 1000,
          "{{nameof(SampleOption.ConnectionString)}}": "bar"
      }
    }
    """)]
    public void Update_Writes_Json_WithBOM(string fileText)
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(fileText, Encoding.UTF8);

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;
        var configStub = new Mock<IConfigurationRoot>();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(new()
        {
            LastLaunchedAt = new(2020, 12, 1),
            Interval = 5000,
            ConnectionString = "foo",
        });

        // Assert
        /*lang=json,strict*/
        const string expectedJson =
        $$"""
        {
          "{{nameof(SampleOption)}}": {
            "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-12-01T00:00:00",
            "{{nameof(SampleOption.Interval)}}": 5000,
            "{{nameof(SampleOption.ConnectionString)}}": "foo"
          }
        }
        """;
        _ = tempFile.ReadAllText(Encoding.UTF8).Should().Be(NormalizeEndLine(expectedJson));
        configStub.Verify(m => m.Reload(), Times.Never());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update(TOptions, bool)"/> does not changes other JSON section.
    /// </summary>
    /// <param name="fileText">Current JSON text</param>
    [Theory(DisplayName = ".Update(TOptions) does not changes other JSON section")]
    [InlineData(/*lang=json,strict*/ "{\"fooOption\":{}}")]
    [InlineData(/*lang=json,strict*/ $$"""
    {
      "fooOption": {},
      "{{nameof(SampleOption)}}": {}
    }
    """)]
    [InlineData(/*lang=json,strict*/ $$"""
    {
      "fooOption": {},
      "{{nameof(SampleOption)}}": {
          "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-10-01T00:00:00",
          "{{nameof(SampleOption.Interval)}}": 1000,
          "{{nameof(SampleOption.ConnectionString)}}": "bar"
      }
    }
    """)]
    public void Update_DoesNot_Changes_Other_Section(string fileText)
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText(fileText);

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub);

        // Act
        sut.Update(new()
        {
            LastLaunchedAt = new(2020, 12, 1),
            Interval = 5000,
            ConnectionString = "foo",
        }, true);

        // Assert
        string newLine = Environment.NewLine;
        _ = tempFile.ReadAllText().Should().Contain("\"fooOption\": {},");
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> calls <see cref="IConfigurationRoot.Reload"/>.
    /// </summary>
    [Fact(DisplayName = ".Update(TOptions, true) calls IConfigurationRoot.Reload()")]
    public void Update_Calls_Reload()
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        tempFile.AppendAllText("{}");

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;
        var configStub = new Mock<IConfigurationRoot>();

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub.Object);

        // Act
        sut.Update(new()
        {
            LastLaunchedAt = new(2020, 12, 1),
            Interval = 5000,
            ConnectionString = "foo",
        }, true);

        // Assert
        /*lang=json,strict*/
        const string expectedJson =
        $$"""
        {
          "{{nameof(SampleOption)}}": {
            "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-12-01T00:00:00",
            "{{nameof(SampleOption.Interval)}}": 5000,
            "{{nameof(SampleOption.ConnectionString)}}": "foo"
          }
        }
        """;
        _ = tempFile.ReadAllText().Should().Be(NormalizeEndLine(expectedJson));
        configStub.Verify(m => m.Reload(), Times.Once());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> creates new JSON file if not exists.
    /// </summary>
    [Fact(DisplayName = ".Update(TOptions, true) creates new JSON file if not exists")]
    public void Update_Creates_File()
    {
        // Arrange
        using var tempFile = new TempFileProvider();
        File.Delete(tempFile.Path);

        var optionsStub = new Mock<IOptionsMonitor<SampleOption>>().Object;
        var configStub = new Mock<IConfigurationRoot>().Object;

        var sut = new JsonWritableOptions<SampleOption>(tempFile.Path, nameof(SampleOption), optionsStub, configStub);

        // Act
        sut.Update(new()
        {
            LastLaunchedAt = new(2020, 12, 1),
            Interval = 5000,
            ConnectionString = "foo",
        });

        // Assert
        /*lang=json,strict*/
        const string expectedJson =
        $$"""
        {
          "{{nameof(SampleOption)}}": {
            "{{nameof(SampleOption.LastLaunchedAt)}}": "2020-12-01T00:00:00",
            "{{nameof(SampleOption.Interval)}}": 5000,
            "{{nameof(SampleOption.ConnectionString)}}": "foo"
          }
        }
        """;
        _ = File.Exists(tempFile.Path).Should().BeTrue();
        _ = tempFile.ReadAllText().Should().Be(NormalizeEndLine(expectedJson));
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
