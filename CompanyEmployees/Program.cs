using CompanyEmployees.Extensions;
using CompanyEmployees.Presentation.ActionFilters;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NLog;

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


builder.Services.AddAutoMapper(typeof(Program));
builder.Services.Configure<ApiBehaviorOptions>(options => // For removing the default model state validation
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddScoped<ValidationFilterAttribute>(); // Action filter for validation
builder.Services.AddControllers(config =>
    {
        config.RespectBrowserAcceptHeader = true; // To support XML formaters
        config.ReturnHttpNotAcceptable = true; // To handle unsupported format
        config.InputFormatters.Insert(0, GetJsonPatchInputFormatter()); //We place the JsonPatchInputFormatter at the index 0
    }).AddXmlDataContractSerializerFormatters()
    .AddCustomCSVFormatter()
    .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);


var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

if(app.Environment.IsProduction())
    app.UseHsts();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection(); // redirection from HTTP to HTTPS
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions() { ForwardedHeaders = ForwardedHeaders.All }); // Forward proxy hearders to the current headers
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers(); //Add endpoints

app.Run(); // Run the application and block the calling thread until the shutdown
