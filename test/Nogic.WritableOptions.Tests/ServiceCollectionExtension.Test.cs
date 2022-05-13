using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nogic.WritableOptions.Tests;

/// <summary>
/// Unit Test Class for <see cref="ServiceCollectionExtension"/>.
/// </summary>
public sealed class ServiceCollectionExtensionTest
{
    /// <summary>
    /// <see cref="ServiceCollectionExtension.ConfigureWritable{TOptions}"/> calls <see cref="ServiceCollectionServiceExtensions.AddTransient{IWritableOptions{TOptions}}(IServiceCollection, Func{IServiceProvider, IWritableOptions{TOptions}})"/>.
    /// </summary>
    [Fact(DisplayName = $"{nameof(ServiceCollectionExtension.ConfigureWritable)}<TOptions>() calls {nameof(ServiceCollectionServiceExtensions.AddTransient)}<IWritableOptions<TOptions>>().")]
    public void ConfigureWritable_Calls_AddTransient()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var service = new ServiceCollection();

        // Act
        service.ConfigureWritable<SampleOption>(configuration.GetSection("Nested"));
        var provider = service.BuildServiceProvider();

        // Assert
        var option = provider.GetService<IWritableOptions<SampleOption>>();
        var otherOption = provider.GetService<IWritableOptions<SampleOption>>();

        _ = option.Should().NotBeNull()
            .And.BeOfType<JsonWritableOptions<SampleOption>>()
            .And.NotBe(otherOption);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtension.ConfigureWritableWithExplicitPath{TOptions}"/> calls <see cref="ServiceCollectionServiceExtensions.AddTransient{IWritableOptions{TOptions}}(IServiceCollection, Func{IServiceProvider, IWritableOptions{TOptions}})"/>.
    /// </summary>
    [Fact(DisplayName = $"{nameof(ServiceCollectionExtension.ConfigureWritableWithExplicitPath)}<TOptions>() calls {nameof(ServiceCollectionServiceExtensions.AddTransient)}<IWritableOptions<TOptions>>().")]
    public void ConfigureWritableWithExplicitPath_Calls_AddTransient()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var service = new ServiceCollection();

        // Act
        service.ConfigureWritableWithExplicitPath<SampleOption>(configuration.GetSection("Nested"), AppDomain.CurrentDomain.BaseDirectory!);
        var provider = service.BuildServiceProvider();

        // Assert
        var option = provider.GetService<IWritableOptions<SampleOption>>();
        var otherOption = provider.GetService<IWritableOptions<SampleOption>>();

        _ = option.Should().NotBeNull()
            .And.BeOfType<JsonWritableOptions<SampleOption>>()
            .And.NotBe(otherOption);
    }
}
