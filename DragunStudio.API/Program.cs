using DragunStudio.API;
using DragunStudio.API.Helpers;
using DragunStudio.API.Models;
using DragunStudio.API.Services;
using DragunStudio.Common;
using DragunStudio.Managers.Common;
using DragunStudio.Managers.Package;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Newtonsoft;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ApplicationSettings applicationSettings = builder.Configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>(); ;
builder.Services.AddSingleton(applicationSettings);
builder.Services.AddSingleton<IConnectionStringFactory, ConnectionStringFactory>();
builder.Services.AddSingleton<ConnectionFactory>();
builder.Services.AddSingleton<PackageManager>();
builder.Services.AddScoped<JWTHandler>();
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{

    config
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
});


WebApplication app = builder.Build();

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
