using DomainLogic.Services;
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

        Assert.NotNull(loggerFactory);
        Assert.NotNull(logger);
    }
}