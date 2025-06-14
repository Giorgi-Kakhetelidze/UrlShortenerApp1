﻿using Cassandra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using UrlShortener.API.Data;
using UrlShortenerApp1.src.UrlShortener.Api.Services.Background;
using UrlShortenerApp1.src.UrlShortener.Api.Services.Implementations;
using UrlShortenerApp1.src.UrlShortener.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<ExpirationCleanupService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "URL Shortener API",
        Version = "v1",
        Description = "API for shortening and managing URLs"
    });
});

var cassandraConnector = new CassandraConnector();
var cassandraSession = cassandraConnector.ConnectAsync().GetAwaiter().GetResult();
builder.Services.AddSingleton<Cassandra.ISession>(cassandraSession);

builder.Services.AddScoped<IUrlService, UrlService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "URL Shortener API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();