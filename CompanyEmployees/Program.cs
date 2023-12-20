using AspNetCoreRateLimit;
using CompanyEmployees.Extensions;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Utility;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NLog;
using Service.DataShaping;
using Shared.DataTransferObjects;

var builder = WebApplication.CreateBuilder(args);

// Add log
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Configure support for JSON Patch using NewtonSoft.Json while leaving the other formatters unchanged(e.g System.Text.Json)
NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
        .Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First();

// Add services to the container.
builder.Services.configureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureServiceNpgsqlContext(builder.Configuration);
builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.Configure<ApiBehaviorOptions>(options => // For removing the default model state validation
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddScoped<ValidationFilterAttribute>(); // Action filter for validation
builder.Services.AddScoped<ValidateMediaTypeAttribute>(); // Actionfilter for mediaType validation for HATEOS
builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>(); // For links generating
builder.Services.ConfigureResponseCaching(); // For native caching
builder.Services.ConfigureHttpCacheHeaders(); // For cashing with marvin 

builder.Services.AddControllers(config =>
    {
        config.RespectBrowserAcceptHeader = true; // To support XML formaters
        config.ReturnHttpNotAcceptable = true; // To handle unsupported format
        config.InputFormatters.Insert(0, GetJsonPatchInputFormatter()); //We place the JsonPatchInputFormatter at the index 0
        config.CacheProfiles.Add("120SecondsDuration", new CacheProfile(){ Duration = 120}); //
    }).AddXmlDataContractSerializerFormatters()
    .AddCustomCSVFormatter()
    .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);
builder.Services.AddCustomMediaTypes();
builder.Services.ConfigureVersioning(); // For versioning management***
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

if(app.Environment.IsProduction())
    app.UseHsts();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection(); // redirection from HTTP to HTTPS
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions() { ForwardedHeaders = ForwardedHeaders.All }); // Forward proxy hearders to the current headers
app.UseIpRateLimiting(); // For the rate limiting
app.UseCors("CorsPolicy");
app.UseResponseCaching(); // For native caching
app.UseHttpCacheHeaders(); // For caching with marvin
app.UseAuthorization();
app.MapControllers(); //Add endpoints

app.Run(); // Run the application and block the calling thread until the shutdown
