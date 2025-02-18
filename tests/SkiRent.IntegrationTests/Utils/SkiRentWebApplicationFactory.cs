using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SkiRent.Api.Data;

namespace SkiRent.IntegrationTests.Utils
{
    public class SkiRentWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private bool _disposed = false;
        private readonly IServiceScope _serviceScope;

        public SkiRentWebApplicationFactory()
        {
            _serviceScope = Services.CreateScope();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<SkiRentContext>));

                if (dbContextDescriptor is not null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var databaseName = Guid.NewGuid().ToString();

                services.AddDbContext<SkiRentContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName);
                });
            });
        }

        public T GetRequiredService<T>()
        {
            return (T)_serviceScope.ServiceProvider.GetRequiredService(typeof(T));
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _serviceScope.Dispose();
            }

            _disposed = true;
            base.Dispose(disposing);
        }
    }
}
