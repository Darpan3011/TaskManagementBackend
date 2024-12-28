using Serilog;
using finalSubmissionDotNet.BuilderExtensions;
using finalSubmissionDotNet.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});

builder.Services.AddServiceCollection(builder.Configuration);

// Build the application
WebApplication app = builder.Build();
app.UseCors("AllowLocalhost4200");

// Middleware setup
app.UseExceptionMiddleware();
app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();