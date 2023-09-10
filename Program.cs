using WeatherAPI;
using WeatherAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WeatherDbContext>(options =>
{
    options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    );
});
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<DistrictDataService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder
            .WithOrigins("https://api.open-meteo.com") 
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

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
