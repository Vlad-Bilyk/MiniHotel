using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniHotel.Infrastructure.Data;
using Testcontainers.PostgreSql;

namespace MiniHotel.Tests.Integration
{
    public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _pgContainer =
                new PostgreSqlBuilder()
                    .WithImage("postgres:16-alpine")
                    .WithDatabase("MiniHotelTest")
                    .WithUsername("postgres")
                    .WithPassword("postgres")
                    .Build();

        public async Task InitializeAsync() =>
            await _pgContainer.StartAsync();

        public new async Task DisposeAsync() =>
            await _pgContainer.DisposeAsync();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultPostgresSQLConnection"] = _pgContainer.GetConnectionString(),
                    ["Jwt:Key"] = "ThisIsASecretKeyForTestingPurposesOnly",
                });
            });

            builder.ConfigureServices((ctx, services) =>
            {
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MiniHotelDbContext>();
                db.Database.Migrate();
            });
        }
    }
}
