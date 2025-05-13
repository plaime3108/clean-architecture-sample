using Application.Interfaces.Services.Login;
using Application.Services.Login;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Infrastructure.Repositories.Login;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using Web.API.CNF.Endpoints;
using Web.API.CNF.Extensions;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(DatabaseSettings.SectionName));
builder.Services.Configure<RedisConfig>(
    builder.Configuration.GetSection(RedisConfig.SectionName));

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var config = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new SqlConnection(config.GetConnectionString());
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConfig = sp.GetRequiredService<IOptions<RedisConfig>>().Value;
    var configOptions = new ConfigurationOptions
    { 
        Password = redisConfig.Password,
        User = redisConfig.Username,
        //AbortOnConnectFail = false,
        //AllowAdmin = true
    };
    configOptions.EndPoints.Add(redisConfig.Hostname ?? string.Empty);

    return ConnectionMultiplexer.Connect(configOptions);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.InstanceName = "CORE_CNF";
});

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();
app.UseHttpsRedirection();
app.MapApiEndpoints();

app.Run();
