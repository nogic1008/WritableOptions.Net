using System;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Nogic.WritableOptions.Tests
{
    public sealed class JsonWritableOptionsTest
    {
        public class SampleOption
        {
            public DateTime LastLaunchedAt { get; set; }
            public int Interval { get; set; }
            public string? ConnectionString { get; set; }
        }

        private static readonly Random _random = new();
        private static SampleOption GenerateOption() => new()
        {
            LastLaunchedAt = DateTime.Now,
            Interval = _random.Next(),
            ConnectionString = new Guid().ToString()
        };

        [Fact]
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

        [Fact]
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

        [Theory]
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
}
