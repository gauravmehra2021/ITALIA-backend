using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SSeuropa.Application.Interfaces;
using SSeuropa.Infrastructure;
using SSeuropa.Infrastructure.Persistence;
using SSeuropa.Infrastructure.Repositories;

namespace SSeuropa.Tests.Infrastructure;

public class MongoDependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_WithMongoProvider_RegistersMongoRepositoriesAndInitializer()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "MongoDb",
                ["Database:DatabaseName"] = "sseuropa_test",
                ["Database:SeedDemoDataOnEmpty"] = "false",
                ["ConnectionStrings:MongoDb"] = "mongodb://localhost:27017/sseuropa_test"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(config);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<IUserRepository>().Should().BeOfType<MongoUserRepository>();
        scope.ServiceProvider.GetRequiredService<IEnquiryRepository>().Should().BeOfType<MongoEnquiryRepository>();
        scope.ServiceProvider.GetRequiredService<ICourseRepository>().Should().BeOfType<MongoCourseRepository>();
        provider.GetServices<IHostedService>().Should().ContainSingle(s => s is MongoDbInitializer);
    }

    [Fact]
    public void AddInfrastructure_WithMongoProviderAndMissingConnectionString_Throws()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "MongoDb"
            })
            .Build();

        var services = new ServiceCollection();
        var act = () => services.AddInfrastructure(config);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*ConnectionStrings:MongoDb*");
    }
}

