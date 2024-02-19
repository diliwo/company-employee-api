using AspNetCoreRateLimit;
using Contracts;
using LoggerService;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;
using System;

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

    // For MediaType
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
                systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.codemaze.apiroot+json");

            }

            // We register a custom media type for XML here.
            var xmlOutputFormatter = config.OutputFormatters
                .OfType<XmlDataContractSerializerInputFormatter>()?
                .FirstOrDefault();

            if (xmlOutputFormatter != null)
            {
                xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.codemaze.hateoas+xml");
                xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.codemaze.apiroot+xml");
            }
        });
    }

    // For Versioning
    public static void ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true; // Adds the API version to the response header.
            opt.AssumeDefaultVersionWhenUnspecified = true; // Specifies the default API version if the client doesn't send one. 
            opt.DefaultApiVersion = new ApiVersion(1, 0); //sets the default version count.
            opt.ApiVersionReader = new QueryStringApiVersionReader("api-version");
        });
    }

    // For native Caching
    public static void ConfigureResponseCaching(this IServiceCollection services) => services.AddResponseCaching();

    // For caching with marvin
    public static void ConfigureHttpCacheHeaders(this IServiceCollection services) => services.AddHttpCacheHeaders(
        (expirationOpt) =>
        {
            expirationOpt.MaxAge = 65; // Set Age to 65 seconds
            expirationOpt.CacheLocation = CacheLocation.Private; // Set to private cache
        },
        (validationOpt) =>
        {
            validationOpt.MustRevalidate = true;
        });

    public static void ConfigureRateLimitingOptions(this IServiceCollection services)
    {
        // Create a rate limit rules
        var rateLimitRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*", // For any endpoints
                Limit = 10, // we stating that 3 requests are allowed
                Period = "5m" // in 5 minutes period
            }
        };

        
        services.Configure<IpRateLimitOptions>(opt => { opt.GeneralRules = rateLimitRules; }); // configure with the rules above
        
        // Serve the purpose of storing rate limit counters and policies as well as adding configuration.
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }
}