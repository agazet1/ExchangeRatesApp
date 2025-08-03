using ExchangeRatesApi;
using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.Services;
using ExchangeRatesApi.Services.Interfaces;
using NLog;
using NLog.Web;


var logger = LogManager
    .Setup()
    .LoadConfigurationFromFile("NLog.config")
    .GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

    var angularOriginName = "AngularOrigin";
    var angularOrigin = builder.Configuration.GetValue<string>("FrontAppUrl")
            ?? throw new InvalidOperationException("Connection string"
            + "'FrontAppUrl' not found.");

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(angularOriginName,
            builder =>
            {
                builder.WithOrigins(angularOrigin)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();

            });
    });

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddHttpClient();

    // Add services to the container.
    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("AppConfiguration"));

    builder.Services.AddSingleton<IAppConfigurationService, AppConfigurationService>();
    builder.Services.AddSingleton<ICacheService, DailyCacheService>();
    builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
    builder.Services.AddScoped<IExchangeRateApiAdapter, ExchangeRateApiAdapter>();
    builder.Services.AddScoped<IExchangeRateApiFactory, ExchangeRateApiFactory>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<RequestResponseLoggingMiddleware>();

    app.UseHttpsRedirection();

    app.UseCors(angularOriginName);

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    logger.Fatal(ex);
    throw;
}
finally
{
    LogManager.Shutdown();
}
