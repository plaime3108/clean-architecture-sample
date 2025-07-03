using Application.Interfaces.Cache;
using Application.Interfaces.External;
using Application.Interfaces.Services.Accounts;
using Application.Interfaces.Services.Credits;
using Application.Interfaces.Services.Login;
using Application.Interfaces.Utils;
using Application.Services.Credits;
using Application.Services.Accounts;
using Application.Services.Login;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Infrastructure.External;
using Infrastructure.Repositories.Common;
using Infrastructure.Repositories.Accounts;
using Infrastructure.Repositories.Login;
using Infrastructure.Repositories.Persons;
using Infrastructure.Services.Cache;
using Infrastructure.Services.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Data;
using System.Net;
using Web.API.CNF.Endpoints;
using Web.API.CNF.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var btConfig = new BtServicesConfig();
builder.Configuration.GetSection(BtServicesConfig.SectionName).Bind(btConfig);

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(DatabaseSettings.SectionName));
builder.Services.Configure<BtServicesConfig>(
    builder.Configuration.GetSection(BtServicesConfig.SectionName));

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

builder.Services.AddHttpClient(btConfig.HttpClientName, (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<BtServicesConfig>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl ?? string.Empty);
    client.Timeout = TimeSpan.FromSeconds(options.Timeout);
}).ConfigurePrimaryHttpMessageHandler(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<BtServicesConfig>>().Value;
    return new HttpClientHandler
    {
        Credentials = new NetworkCredential(options.Username, options.Password),
        PreAuthenticate = true,
        UseDefaultCredentials = false,
        AllowAutoRedirect = false,
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
});

builder.Services.AddScoped<ILoginServices, LoginService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IListAccountsServices, ListAccountsServices>();
builder.Services.AddScoped<IAccountsRepository, AccountsRepository>();
builder.Services.AddScoped<IAccountTransactionsServices, AccountTransactionsServices>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<IGetDetailCreditServices, GetDetailCreditServices>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ICommonRepository, CommonRepository>();
builder.Services.AddScoped<IExternalApiClient, GetBtService>();
builder.Services.AddScoped<IBtinreqProvider, BtinreqProvider>();

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
