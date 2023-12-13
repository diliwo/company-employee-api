using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;

namespace CompanyEmployees.Extensions;

public static class ServiceExtensions
{
    public static void configureCors(this IServiceCollection services) => services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination")); // Enable the client to read the new X-Pagination header
    });

    // For hosting on IIS
    public static void ConfigureIISIntegration(this IServiceCollection services) => 
        services.Configure<IISOptions>(options =>
        {

        });

    // For Logger
    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    // For RepositoryManager
    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    // For ServiceManager
    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();

    // For RepositoryContext
    //public static void ConfigureServiceNpgsqlContext(this IServiceCollection services, IConfiguration configuration) =>
    //    services.AddNpgsql<RepositoryContext>((configuration.GetConnectionString("NpgsqlConnection")));

    public static void ConfigureServiceNpgsqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts => opts.UseNpgsql(configuration.GetConnectionString("NpgsqlConnection")));

    // For custom Csv formater 
    public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
        builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutPutFormatter()));

    public static void AddCustomMediaTypes(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(config =>
        {
            // We register a custom media type for JSON here.
            var systemTextJsonOutputFormatter =
                config.OutputFormatters.OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();

            if (systemTextJsonOutputFormatter != null)
            {
                systemTextJsonOutputFormatter .SupportedMediaTypes
                    .Add("application/vnd.codemaze.hateoas+json");
            }

            // We register a custom media type for XML here.
            var xmlOutputFormatter = config.OutputFormatters
                .OfType<XmlDataContractSerializerInputFormatter>()?
                .FirstOrDefault();

            if (xmlOutputFormatter != null)
            {
                xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.codemaze.hateoas+xml");
            }
        });
    }
}