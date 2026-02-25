var builder = DistributedApplication.CreateBuilder(args);

// Add Elasticsearch
var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Add Identity Service
var identity = builder.AddProject<Projects.Play_Identity>("identity")
    .WithExternalHttpEndpoints();

// Add Catalog Service with Elasticsearch
var catalog = builder.AddProject<Projects.Play_Catalog>("catalog")
    .WithReference(elasticsearch)
    .WithReference(identity)
    .WithExternalHttpEndpoints();

// Add Payment Service
var payment = builder.AddProject<Projects.Play_Payment>("payment")
    .WithReference(identity)
    .WithExternalHttpEndpoints();

builder.Build().Run();

