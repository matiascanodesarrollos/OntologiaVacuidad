using DomainLogic.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDharmaServices_RegistraServiciosDeLogging()
    {
        var services = new ServiceCollection();

        services.AddDharmaServices();
        using var provider = services.BuildServiceProvider();

        var loggerFactory = provider.GetService<ILoggerFactory>();
        var logger = provider.GetService<ILogger<ServiceCollectionExtensionsTests>>();

        loggerFactory.Should().NotBeNull();
        logger.Should().NotBeNull();
    }
}