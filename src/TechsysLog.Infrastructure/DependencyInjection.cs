using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TechsysLog.Application.Abstractions.Authentication;
using TechsysLog.Application.Abstractions.DomainServices;
using TechsysLog.Application.Abstractions.Events;
using TechsysLog.Application.Abstractions.ExternalServices;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Application.Abstractions.Realtime;
using TechsysLog.Domain.Common;
using TechsysLog.Infrastructure.Authentication;
using TechsysLog.Infrastructure.Events;
using TechsysLog.Infrastructure.ExternalServices.ViaCep;
using TechsysLog.Infrastructure.Persistence.Mongo;
using TechsysLog.Infrastructure.Persistence.Mongo.Counters;
using TechsysLog.Infrastructure.Persistence.Mongo.Indexes;
using TechsysLog.Infrastructure.Persistence.Mongo.Repositories;
using TechsysLog.Infrastructure.Realtime;
using TechsysLog.Infrastructure.Time;

namespace TechsysLog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Settings
        services.Configure<MongoSettings>(configuration.GetSection(MongoSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<ViaCepSettings>(configuration.GetSection(ViaCepSettings.SectionName));

        // Mongo (singleton client + scoped context)
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });
        services.AddScoped<MongoContext>();
        services.AddHostedService<MongoIndexInitializer>();

        // Persistence
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IOrderNumberGenerator, MongoOrderNumberGenerator>();

        // Auth
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        // Time
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        // ViaCEP HttpClient + cache + resilience
        services.AddMemoryCache();
        services.AddHttpClient<ICepLookupService, ViaCepClient>((sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<ViaCepSettings>>().Value;
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        })
        .AddStandardResilienceHandler();   // retry exponencial + circuit breaker

        // Realtime
        services.AddSignalR();
        services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();

        // Domain events
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        RegisterDomainEventHandlers(services);

        return services;
    }

    private static void RegisterDomainEventHandlers(IServiceCollection services)
    {
        // Auto-registra IDomainEventHandler<T> implementations da Application
        var applicationAssembly = typeof(Application.DependencyInjection).Assembly;
        var handlerInterface = typeof(IDomainEventHandler<>);

        var implementations = applicationAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Select(t => new
            {
                Implementation = t,
                Interfaces = t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface)
                    .ToList()
            })
            .Where(x => x.Interfaces.Count > 0);

        foreach (var item in implementations)
            foreach (var iface in item.Interfaces)
                services.AddScoped(iface, item.Implementation);
    }
}
