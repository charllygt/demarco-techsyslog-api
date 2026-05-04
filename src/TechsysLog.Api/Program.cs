using Scalar.AspNetCore;
using Serilog;
using TechsysLog.Api.Configuration;
using TechsysLog.Api.Middleware;
using TechsysLog.Application;
using TechsysLog.Infrastructure;
using TechsysLog.Infrastructure.Realtime;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiLayer(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechsysLog API v1"));
    app.MapScalarApiReference();
}

app.UseCors(CorsSetup.PolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();

// Permite que WebApplicationFactory<Program> funcione nos integration tests
public partial class Program;
