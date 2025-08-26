using Microsoft.AspNetCore.Localization;
using SimuladorCredito.Repositories;
using SimuladorCredito.Services;
using SimuladorCredito.Services.Cache;
using System.Globalization;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using SimuladorCredito.Util;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("SimuladorCredito"))
            .AddAspNetCoreInstrumentation();
    });

builder.Services.AddSingleton<TelemetryService>();



// Configurar a cultura padrão para ISO 8601
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSingleton<SimulacaoContext>();
builder.Services.AddSingleton<SimulacaoRepository>();
builder.Services.AddSingleton<DbHackaThonContext>();
builder.Services.AddSingleton<ProdutosStaticService>();
builder.Services.AddSingleton<CalculoSimulacaoService>();



builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("Database Simulacao Connection")
    .AddCheck<DbHackaThonHealthCheck>("Database HackaThon Connection");



var app = builder.Build();

var defaultCulture = "pt-BR";
var cultureInfo = new CultureInfo(defaultCulture);
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = new List<CultureInfo> { cultureInfo },
    SupportedUICultures = new List<CultureInfo> { cultureInfo }
});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.Use(async (context, next) =>
{
    var telemetry = context.RequestServices.GetRequiredService<SimuladorCredito.Services.TelemetryService>();
    var start = DateTime.UtcNow;
    await next.Invoke();
    var duration = (DateTime.UtcNow - start).TotalMilliseconds;
    var statusCode = context.Response.StatusCode;
    var path = context.Request.Path;
    var method = context.Request.Method;
    telemetry.IncrementRequestCount();
    telemetry.RegisterRequest(start, duration, statusCode, path, method);
});



app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
