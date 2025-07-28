using Data.Contexts;
using Data.Repositories;
using Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Infrastructure
{
    public static class DataServiceProvider
    {
        private static ServiceProvider? _provider;

        public static void Initialize()
        {
            var configuration = ConfigurationLoader.Load();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var services = new ServiceCollection();

            // Подключаем DbContext с полученной строкой подключения
            services.AddDbContext<BlazorBankContext>(options =>
                options.UseSqlServer(connectionString));

            // Регистрируем репозитории и службы
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ClientService>();

            _provider = services.BuildServiceProvider();
        }

        public static T GetService<T>() where T : notnull
        {
            if (_provider == null)
                throw new InvalidOperationException("DataServiceProvider is not initialized.");

            return _provider.GetRequiredService<T>();
        }
    }
}
