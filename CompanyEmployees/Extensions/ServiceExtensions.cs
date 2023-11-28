using Contracts;
using LoggerService;
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
                .AllowAnyHeader());
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
}