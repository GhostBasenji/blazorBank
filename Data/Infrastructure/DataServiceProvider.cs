using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Data.Contexts;

namespace Data.Infrastructure;

public static class DataServiceProvider
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<BlazorBankContext>(options =>
            options.UseSqlServer(connectionString));

        // Здесь можешь добавить другие сервисы, например, репозитории
        return services;
    }
}
