using Data.Contexts;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Infrastructure
{
    public static class DataServiceProvider
    {
        private static IServiceProvider? _provider;

        public static void Initialize()
        {
            var config = ConfigurationLoader.Load();
            var connectionString = config.GetConnectionString("DefaultConnection");

            var services = new ServiceCollection();

            // Регистрация DbContext
            services.AddDbContext<BlazorBankContext>(options =>
                options.UseSqlServer(connectionString));

            // Регистрация репозиториев
            services.AddScoped<IClientRepository, ClientRepository>();

            // В будущем можно регистрировать и сервисы:
            // services.AddScoped<IClientService, ClientService>();

            _provider = services.BuildServiceProvider();
        }

        public static T GetService<T>() => _provider!.GetRequiredService<T>();
    }
}
