using Microsoft.AspNetCore.Authentication.JwtBearer;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Play.Catalog.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Elasticsearch
var elasticsearchUrl = builder.Configuration.GetConnectionString("elasticsearch") ?? "http://localhost:9200";
var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
    .DefaultIndex("catalog-items");

builder.Services.AddSingleton(new ElasticsearchClient(settings));
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var identityUrl = builder.Configuration.GetConnectionString("identity") ?? "https://localhost:7164";
        options.Authority = identityUrl;
        options.Audience = "catalog";
        options.RequireHttpsMetadata = false; // For development
    });

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public record PublicKeyResponse(string PublicKey);

