using System;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Nogic.WritableOptions.Tests
{
    public sealed class JsonWritableOptionsTest
    {
        public class TestOption
        {
            public DateTime LastLaunchedAt { get; set; }
            public int Interval { get; set; }
            public string? ConnectionString { get; set; }
        }

        private static readonly Random _random = new();
        private static TestOption GenerateOption() => new()
        {
            LastLaunchedAt = DateTime.Now,
            Interval = _random.Next(),
            ConnectionString = new Guid().ToString()
        };

        [Fact]
        public void Value_Returns_T()
        {
            // Arrange
            var testOption = GenerateOption();
            var optionsMock = new Mock<IOptionsMonitor<TestOption>>();
            optionsMock.SetupGet(m => m.CurrentValue).Returns(testOption);

            var options = new JsonWritableOptions<TestOption>(null!, optionsMock.Object, null!, null!);

            // Act - Assert
            options.Value.Should().Be(testOption);
        }

        [Fact]
        public void Get_Returns_T()
        {
            // Arrange
            var testOption = GenerateOption();
            var optionsMock = new Mock<IOptionsMonitor<TestOption>>();
            optionsMock.Setup(m => m.Get(It.IsAny<string>())).Returns(testOption);

            var options = new JsonWritableOptions<TestOption>(null!, optionsMock.Object, nameof(TestOption), null!);

            // Act - Assert
            options.Get("Foo").Should().Be(testOption);
            options.Get("Bar").Should().Be(testOption);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"" + nameof(TestOption) + "\":{}}")]
        [InlineData("{\"" + nameof(TestOption) + "\":{\"LastLaunchedAt\":\"2020-10-01T00:00:00\",\"Interval\":1000,\"ConnectionString\":\"bar\"}}")]
        public void Update_Writes_Json(string fileText)
        {
            // Setup
            string tempFilePath = Path.GetTempFileName();
            File.AppendAllText(tempFilePath, fileText);
            string tempFileName = Path.GetFileName(tempFilePath);

            try
            {
                // Arrange
                var environmentMock = new Mock<IHostEnvironment>();
                environmentMock.SetupGet(m => m.ContentRootFileProvider.GetFileInfo(tempFileName).PhysicalPath).Returns(tempFilePath);
                var optionsMock = new Mock<IOptionsMonitor<TestOption>>();

                var options = new JsonWritableOptions<TestOption>(environmentMock.Object, optionsMock.Object, nameof(TestOption), tempFileName);

                // Act
                options.Update(new()
                {
                    LastLaunchedAt = new(2020, 12, 1),
                    Interval = 5000,
                    ConnectionString = "foo",
                });

                // Assert
                string newLine = Environment.NewLine;
                string jsonString = File.ReadAllText(tempFilePath);
                jsonString.Should().Be("{" + newLine
                    + "  \"TestOption\": {" + newLine
                    + "    \"LastLaunchedAt\": \"2020-12-01T00:00:00\"," + newLine
                    + "    \"Interval\": 5000," + newLine
                    + "    \"ConnectionString\": \"foo\"" + newLine
                    + "  }" + newLine
                    + "}"
                );
            }
            finally
            {
                // Teardown
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }
    }
}
