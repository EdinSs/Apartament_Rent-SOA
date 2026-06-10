using EasyRent.Application.Interfaces.Repositories;
using EasyRent.Infrastructure.Data;
using EasyRent.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyRent.Infrastructure;

/// <summary>
/// Registers all Infrastructure services (DbContext + repositories) in one call.
/// The API's Program.cs invokes <c>services.AddInfrastructure(configuration)</c>, keeping the
/// composition root clean and binding interfaces to implementations via Dependency Injection.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("Default")));

        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}
