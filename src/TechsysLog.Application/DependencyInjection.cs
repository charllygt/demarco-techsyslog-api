using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Behaviors;
using TechsysLog.Application.Messaging;

namespace TechsysLog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddScoped<IDispatcher, Dispatcher>();

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        // Pipeline behaviors são registrados como open generics — ordem importa: Logging > Validation > Handler
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Auto-registra todos os Command/Query handlers do assembly via convenção
        RegisterHandlers(services, assembly, typeof(ICommandHandler<>));
        RegisterHandlers(services, assembly, typeof(ICommandHandler<,>));
        RegisterHandlers(services, assembly, typeof(IQueryHandler<,>));

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly, Type openGeneric)
    {
        var implementations = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Select(t => new
            {
                Implementation = t,
                Interfaces = t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric)
                    .ToList()
            })
            .Where(x => x.Interfaces.Count > 0);

        foreach (var item in implementations)
        {
            foreach (var iface in item.Interfaces)
            {
                services.AddScoped(iface, item.Implementation);
            }
        }
    }
}
