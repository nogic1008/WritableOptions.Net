using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nogic.WritableOptions.Tests;

/// <summary>
/// Unit Test Class for <see cref="ServiceCollectionExtension"/>.
/// </summary>
[TestClass]
public sealed class ServiceCollectionExtensionTest
{
    /// <summary>
    /// <see cref="ServiceCollectionExtension.ConfigureWritable{TOptions}(IServiceCollection, IConfigurationSection, string)"/> calls <see cref="ServiceCollectionServiceExtensions.AddTransient{IWritableOptions{TOptions}}(IServiceCollection, Func{IServiceProvider, IWritableOptions{TOptions}})"/>.
    /// </summary>
    [TestMethod($"{nameof(ServiceCollectionExtension.ConfigureWritable)}<TOptions>() calls {nameof(ServiceCollectionServiceExtensions.AddTransient)}<IWritableOptions<TOptions>>().")]
    public void ConfigureWritable_Calls_AddTransient()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var service = new ServiceCollection();

        // Act
        _ = service.ConfigureWritable<SampleOption>(configuration.GetSection("Nested"));
        var provider = service.BuildServiceProvider();

        // Assert
        var option = provider.GetService<IWritableOptions<SampleOption>>();
        var otherOption = provider.GetService<IWritableOptions<SampleOption>>();

        _ = option.Should().NotBeNull()
            .And.BeOfType<JsonWritableOptions<SampleOption>>()
            .And.NotBe(otherOption);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtension.ConfigureWritable{TOptions}(IServiceCollection, IConfigurationSection, JsonSerializerOptions, string)"/> calls <see cref="ServiceCollectionServiceExtensions.AddTransient{IWritableOptions{TOptions}}(IServiceCollection, Func{IServiceProvider, IWritableOptions{TOptions}})"/>.
    /// </summary>
    [TestMethod($"{nameof(ServiceCollectionExtension.ConfigureWritable)}<TOptions>({nameof(JsonSerializerOptions)}) calls {nameof(ServiceCollectionServiceExtensions.AddTransient)}<IWritableOptions<TOptions>>().")]
    public void ConfigureWritable_JsonSerializerOptions_Calls_AddTransient()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var service = new ServiceCollection();
        var options = new JsonSerializerOptions();

        // Act
        _ = service.ConfigureWritable<SampleOption>(configuration.GetSection("Nested"), options);
        var provider = service.BuildServiceProvider();

        // Assert
        var option = provider.GetService<IWritableOptions<SampleOption>>();
        var otherOption = provider.GetService<IWritableOptions<SampleOption>>();

        _ = option.Should().NotBeNull()
            .And.BeOfType<JsonWritableOptions<SampleOption>>()
            .And.NotBe(otherOption);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtension.ConfigureWritable{TOptions}(IServiceCollection, IConfigurationSection, Func{JsonSerializerOptions}, string)"/> calls <see cref="ServiceCollectionServiceExtensions.AddTransient{IWritableOptions{TOptions}}(IServiceCollection, Func{IServiceProvider, IWritableOptions{TOptions}})"/>.
    /// </summary>
    [TestMethod($"{nameof(ServiceCollectionExtension.ConfigureWritable)}<TOptions>({nameof(Func<JsonSerializerOptions>)}) calls {nameof(ServiceCollectionServiceExtensions.AddTransient)}<IWritableOptions<TOptions>>().")]
    public void ConfigureWritable_FuncJsonSerializerOptions_Calls_AddTransient()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var service = new ServiceCollection();

        // Act
        _ = service.ConfigureWritable<SampleOption>(configuration.GetSection("Nested"), () => new());
        var provider = service.BuildServiceProvider();

        // Assert
        var option = provider.GetService<IWritableOptions<SampleOption>>();
        var otherOption = provider.GetService<IWritableOptions<SampleOption>>();

        _ = option.Should().NotBeNull()
            .And.BeOfType<JsonWritableOptions<SampleOption>>()
            .And.NotBe(otherOption);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtension.ConfigureWritableWithExplicitPath{TOptions}(IServiceCollection, IConfigurationSection, string, string)"/> calls <see cref="ServiceCollectionServiceExtensions.AddTransient{IWritableOptions{TOptions}}(IServiceCollection, Func{IServiceProvider, IWritableOptions{TOptions}})"/>.
    /// </summary>
    [TestMethod($"{nameof(ServiceCollectionExtension.ConfigureWritableWithExplicitPath)}<TOptions>() calls {nameof(ServiceCollectionServiceExtensions.AddTransient)}<IWritableOptions<TOptions>>().")]
    public void ConfigureWritableWithExplicitPath_Calls_AddTransient()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var service = new ServiceCollection();

        // Act
        _ = service.ConfigureWritableWithExplicitPath<SampleOption>(configuration.GetSection("Nested"), AppDomain.CurrentDomain.BaseDirectory!);
        var provider = service.BuildServiceProvider();

        // Assert
        var option = provider.GetService<IWritableOptions<SampleOption>>();
        var otherOption = provider.GetService<IWritableOptions<SampleOption>>();

        _ = option.Should().NotBeNull()
            .And.BeOfType<JsonWritableOptions<SampleOption>>()
            .And.NotBe(otherOption);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtension.ConfigureWritableWithExplicitPath{TOptions}(IServiceCollection, IConfigurationSection, string, JsonSerializerOptions, string)"/> calls <see cref="ServiceCollectionServiceExtensions.AddTransient{IWritableOptions{TOptions}}(IServiceCollection, Func{IServiceProvider, IWritableOptions{TOptions}})"/>.
    /// </summary>
    [TestMethod($"{nameof(ServiceCollectionExtension.ConfigureWritableWithExplicitPath)}<TOptions>({nameof(JsonSerializerOptions)}) calls {nameof(ServiceCollectionServiceExtensions.AddTransient)}<IWritableOptions<TOptions>>().")]
    public void ConfigureWritableWithExplicitPath_JsonSerializerOptions_Calls_AddTransient()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var service = new ServiceCollection();
        var options = new JsonSerializerOptions();

        // Act
        _ = service.ConfigureWritableWithExplicitPath<SampleOption>(configuration.GetSection("Nested"), AppDomain.CurrentDomain.BaseDirectory!, options);
        var provider = service.BuildServiceProvider();

        // Assert
        var option = provider.GetService<IWritableOptions<SampleOption>>();
        var otherOption = provider.GetService<IWritableOptions<SampleOption>>();

        _ = option.Should().NotBeNull()
            .And.BeOfType<JsonWritableOptions<SampleOption>>()
            .And.NotBe(otherOption);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtension.ConfigureWritableWithExplicitPath{TOptions}(IServiceCollection, IConfigurationSection, string, Func{JsonSerializerOptions}, string)"/> calls <see cref="ServiceCollectionServiceExtensions.AddTransient{IWritableOptions{TOptions}}(IServiceCollection, Func{IServiceProvider, IWritableOptions{TOptions}})"/>.
    /// </summary>
    [TestMethod($"{nameof(ServiceCollectionExtension.ConfigureWritableWithExplicitPath)}<TOptions>({nameof(Func<JsonSerializerOptions>)}) calls {nameof(ServiceCollectionServiceExtensions.AddTransient)}<IWritableOptions<TOptions>>().")]
    public void ConfigureWritableWithExplicitPath_FuncJsonSerializerOptions_Calls_AddTransient()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var service = new ServiceCollection();

        // Act
        _ = service.ConfigureWritableWithExplicitPath<SampleOption>(configuration.GetSection("Nested"), AppDomain.CurrentDomain.BaseDirectory!, () => new());
        var provider = service.BuildServiceProvider();

        // Assert
        var option = provider.GetService<IWritableOptions<SampleOption>>();
        var otherOption = provider.GetService<IWritableOptions<SampleOption>>();

        _ = option.Should().NotBeNull()
            .And.BeOfType<JsonWritableOptions<SampleOption>>()
            .And.NotBe(otherOption);
    }
}
