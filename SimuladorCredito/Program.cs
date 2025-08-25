using Microsoft.AspNetCore.Localization;
using SimuladorCredito.Repositories;
using SimuladorCredito.Services;
using SimuladorCredito.Services.Cache;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);


// Configurar a cultura padrão para ISO 8601
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSingleton<SimulacaoContext>();
builder.Services.AddSingleton<SimulacaoRepository>();
builder.Services.AddSingleton<DbHackaThonContext>();
builder.Services.AddSingleton<ProdutosStaticService>();
builder.Services.AddSingleton<CalculoSimulacaoService>();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
