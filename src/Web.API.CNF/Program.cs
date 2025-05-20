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
using Infrastructure.Repositories.ListAccounts;
using Infrastructure.Repositories.Common;
using Application.Interfaces.Services.Accounts;
using Application.Services.ListAccounts;
using Infrastructure.Services.Cache;
using Application.Contracts.Cache;

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

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConnectionMultiplexerFactory = async () =>
    {
        var redisConfig = builder.Configuration.GetSection(RedisConfig.SectionName).Get<RedisConfig>();
        var configOptions = new ConfigurationOptions
        {
            Password = redisConfig!.Password,
            User = redisConfig.Username,
            AbortOnConnectFail = false,
            AllowAdmin = true,
        };
        configOptions.EndPoints.Add(redisConfig.Hostname ?? string.Empty);
        return await ConnectionMultiplexer.ConnectAsync(configOptions);
    };

    options.InstanceName = "CORE_CNF";
});

builder.Services.AddScoped<ILoginServices, LoginService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IListAccountsServices, ListAccountsServices>();
builder.Services.AddScoped<IListAccountsRepository, ListAccountsRepository>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ICommonRepository, CommonRepository>();
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
