using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Nogic.WritableOptions.Tests
{
    public class ServiceCollectionExtensionTest
    {
        public class SampleOption
        {
            public DateTime LastLaunchedAt { get; set; }
            public int Interval { get; set; }
            public string? ConnectionString { get; set; }
        }

        public void ConfigureWritable_Calls_DI()
        {
            // Arrange
            var serviceStub = new Mock<IServiceCollection>();
            var sectionStub = new Mock<IConfigurationSection>();

            // Act
            serviceStub.Object.ConfigureWritable<SampleOption>(sectionStub.Object);

            // Assert
            sectionStub.VerifyGet(m => m.Key, Times.AtLeastOnce());
        }
    }
}
