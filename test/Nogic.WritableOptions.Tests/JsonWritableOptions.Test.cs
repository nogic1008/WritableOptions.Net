namespace Nogic.WritableOptions.Tests;

using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;

/// <summary>
/// Unit Test Class for <see cref="JsonWritableOptions{TOptions}"/>.
/// </summary>
public sealed class JsonWritableOptionsTest
{
    private static readonly Random _random = new();
    /// <summary>
    /// Generates <see cref="SampleOption"/> randomly for test.
    /// </summary>
    private static SampleOption GenerateOption() => new()
    {
        LastLaunchedAt = DateTime.Now,
        Interval = _random.Next(),
        ConnectionString = new Guid().ToString()
    };

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Value"/> returns TOptions via <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    [Fact(DisplayName = ".Value returns TOptions via IOptionsMonitor<TOptions>")]
    public void Value_Returns_T()
    {
        // Arrange
        var sampleOption = GenerateOption();
        var optionsMock = new Mock<IOptionsMonitor<SampleOption>>();
        optionsMock.SetupGet(m => m.CurrentValue).Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>(null!, null!, optionsMock.Object, null);

        // Act - Assert
        sut.Value.Should().Be(sampleOption);
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
        optionsMock.Setup(m => m.Get(It.IsAny<string>())).Returns(sampleOption);

        var sut = new JsonWritableOptions<SampleOption>(null!, null!, optionsMock.Object, null);

        // Act - Assert
        sut.Get("Foo").Should().Be(sampleOption);
        sut.Get("Bar").Should().Be(sampleOption);

        optionsMock.Verify(m => m.Get(It.IsAny<string>()), Times.Exactly(2));
        optionsMock.Verify(m => m.Get("Foo"), Times.Once());
        optionsMock.Verify(m => m.Get("Bar"), Times.Once());
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> writes expected JSON.
    /// </summary>
    /// <param name="fileText">Current JSON text</param>
    [Theory(DisplayName = ".Update(TOptions) writes expected JSON")]
    [InlineData("{}")]
    [InlineData("{\"" + nameof(SampleOption) + "\":{}}")]
    [InlineData("{\"" + nameof(SampleOption) + "\":{\"LastLaunchedAt\":\"2020-10-01T00:00:00\",\"Interval\":1000,\"ConnectionString\":\"bar\"}}")]
    public void Update_Writes_Json(string fileText)
    {
        // Setup
        string tempFilePath = Path.GetTempFileName();
        File.AppendAllText(tempFilePath, fileText);
        string tempFileName = Path.GetFileName(tempFilePath);

        try
        {
            // Arrange
            var configStub = new Mock<IConfigurationRoot>();

            var sut = new JsonWritableOptions<SampleOption>(tempFilePath, nameof(SampleOption), null!, configStub.Object);

            // Act
            sut.Update(new()
            {
                LastLaunchedAt = new(2020, 12, 1),
                Interval = 5000,
                ConnectionString = "foo",
            });

            // Assert
            string newLine = Environment.NewLine;
            string jsonString = File.ReadAllText(tempFilePath);
            jsonString.Should().Be("{" + newLine
                + "  \"" + nameof(SampleOption) + "\": {" + newLine
                + "    \"LastLaunchedAt\": \"2020-12-01T00:00:00\"," + newLine
                + "    \"Interval\": 5000," + newLine
                + "    \"ConnectionString\": \"foo\"" + newLine
                + "  }" + newLine
                + "}"
            );
            configStub.Verify(m => m.Reload(), Times.Never());
        }
        finally
        {
            // Teardown
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update(TOptions, bool)"/> does not changes other JSON section.
    /// </summary>
    /// <param name="fileText">Current JSON text</param>
    [Theory(DisplayName = ".Update(TOptions) does not changes other JSON section")]
    [InlineData("{\"fooOption\":{}}")]
    [InlineData("{\"fooOption\":{},\"" + nameof(SampleOption) + "\":{}}")]
    [InlineData("{\"fooOption\":{},\"" + nameof(SampleOption) + "\":{\"LastLaunchedAt\":\"2020-10-01T00:00:00\",\"Interval\":1000,\"ConnectionString\":\"bar\"}}")]
    public void Update_DoesNot_Changes_Other_Section(string fileText)
    {
        // Setup
        string tempFilePath = Path.GetTempFileName();
        File.AppendAllText(tempFilePath, fileText);
        string tempFileName = Path.GetFileName(tempFilePath);

        try
        {
            // Arrange
            var sut = new JsonWritableOptions<SampleOption>(tempFilePath, nameof(SampleOption), null!);

            // Act
            sut.Update(new()
            {
                LastLaunchedAt = new(2020, 12, 1),
                Interval = 5000,
                ConnectionString = "foo",
            }, true);

            // Assert
            string newLine = Environment.NewLine;
            string jsonString = File.ReadAllText(tempFilePath);
            jsonString.Should().Contain("\"fooOption\": {},");
        }
        finally
        {
            // Teardown
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
    }

    /// <summary>
    /// <see cref="JsonWritableOptions{TOptions}.Update"/> calls <see cref="IConfigurationRoot.Reload"/>.
    /// </summary>
    [Fact(DisplayName = ".Update(TOptions, true) calls IConfigurationRoot.Reload()")]
    public void Update_Calls_Reload()
    {
        // Setup
        string tempFilePath = Path.GetTempFileName();
        File.AppendAllText(tempFilePath, "{}");
        string tempFileName = Path.GetFileName(tempFilePath);

        try
        {
            // Arrange
            var configStub = new Mock<IConfigurationRoot>();

            var sut = new JsonWritableOptions<SampleOption>(tempFilePath, nameof(SampleOption), null!, configStub.Object);

            // Act
            sut.Update(new()
            {
                LastLaunchedAt = new(2020, 12, 1),
                Interval = 5000,
                ConnectionString = "foo",
            }, true);

            // Assert
            string newLine = Environment.NewLine;
            string jsonString = File.ReadAllText(tempFilePath);
            jsonString.Should().Be("{" + newLine
                + "  \"" + nameof(SampleOption) + "\": {" + newLine
                + "    \"LastLaunchedAt\": \"2020-12-01T00:00:00\"," + newLine
                + "    \"Interval\": 5000," + newLine
                + "    \"ConnectionString\": \"foo\"" + newLine
                + "  }" + newLine
                + "}"
            );
            configStub.Verify(m => m.Reload(), Times.Once());
        }
        finally
        {
            // Teardown
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
    }
}
