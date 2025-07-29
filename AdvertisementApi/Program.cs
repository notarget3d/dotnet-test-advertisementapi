using AdvertisementApi.Core;
using AdvertisementApi.Core.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.AddScoped<IAdvertisementModelFactory, AdvertisementModelFactory>();
builder.Services.AddScoped<IAdvertisementDataParser, AdvertisementDataParser>();
builder.Services.AddSingleton<IAdvertisementModelProvider, AdvertisementModelProvider>();

// Init config
builder.Services.Configure<AdvertisementConfig>(builder.Configuration.GetSection(nameof(AdvertisementConfig)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
