using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Infrastructure.Persistence;

namespace TeamOps.Api.Tests;

public sealed class TeamOpsApiFactory : WebApplicationFactory<Program>
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=teamops_test;Username=postgres;Password=mysecretpassword";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<TeamOpsDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TeamOpsDbContext>(options => options.UseNpgsql(ConnectionString));
        });
    }
}