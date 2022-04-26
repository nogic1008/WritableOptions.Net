using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nogic.WritableOptions.Tests;

/// <summary>
/// Unit Test Class for <see cref="ServiceCollectionExtension"/>.
/// </summary>
public sealed class ServiceCollectionExtensionTest
{
    [Fact]
    public void ConfigureWritable_Calls_AddTransient()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var service = new ServiceCollection();

        service.ConfigureWritable<SampleOption>(configuration.GetSection("Nested"));

        var provider = service.BuildServiceProvider();
        var option = provider.GetService<IWritableOptions<SampleOption>>();
        var otherOption = provider.GetService<IWritableOptions<SampleOption>>();

        _ = option.Should().NotBeNull()
            .And.BeOfType<JsonWritableOptions<SampleOption>>()
            .And.NotBe(otherOption);
    }

    [Fact]
    public void ConfigureWritableWithExplicitPath_Calls_AddTransient()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var service = new ServiceCollection();

        service.ConfigureWritableWithExplicitPath<SampleOption>(configuration.GetSection("Nested"), AppDomain.CurrentDomain.BaseDirectory!);

        var provider = service.BuildServiceProvider();
        var option = provider.GetService<IWritableOptions<SampleOption>>();
        var otherOption = provider.GetService<IWritableOptions<SampleOption>>();

        _ = option.Should().NotBeNull()
            .And.BeOfType<JsonWritableOptions<SampleOption>>()
            .And.NotBe(otherOption);
    }
}
