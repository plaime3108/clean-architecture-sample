using Application.Interfaces.Services.Login;
using Application.Services.Login;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Login;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using Web.API.CNF.Endpoints;
using Web.API.CNF.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(DatabaseSettings.SectionName));

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var config = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new SqlConnection(config.GetConnectionString());
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
