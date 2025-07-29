using Data.Contexts;
using Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DataServiceProvider
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<BlazorBankContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IClientAuthService, ClientAuthService>();

        return services;
    }
}
