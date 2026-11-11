using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WebAPI.Data;
using WebAPI.Endpoints;
using WebAPI.Repositories;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<KinetoPlusDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KinetoPlus")));
builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
builder.Services.AddScoped<IClinicService, ClinicService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapClinicEndpoints();

app.Run();
