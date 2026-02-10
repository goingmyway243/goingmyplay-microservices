using Microsoft.AspNetCore.Authentication.JwtBearer;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Play.Catalog.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Elasticsearch
var elasticsearchUrl = builder.Configuration.GetValue<string>("Elasticsearch:Url") ?? "http://localhost:9200";
var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
    .DefaultIndex("catalog-items");

builder.Services.AddSingleton(new ElasticsearchClient(settings));
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7164"; // IdentityServer URL
        options.Audience = "catalog";
        options.RequireHttpsMetadata = false; // For development
    });

var app = builder.Build();



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
