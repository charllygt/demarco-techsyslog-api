using TechsysLog.Api.Authentication;
using TechsysLog.Api.Middleware;
using TechsysLog.Application.Abstractions.Authentication;

namespace TechsysLog.Api.Configuration;

internal static class ApiServicesSetup
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddJwtAuth(configuration);
        services.AddSwaggerWithJwt();
        services.AddDefaultCors(configuration);

        return services;
    }
}
