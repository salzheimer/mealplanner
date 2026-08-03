using GatewayBff;
using GatewayBff.Services;
using HotChocolate.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IServiceClientFactory, ServiceClientFactory>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithName("Health");

// Banana Cake Pop (the GraphQL IDE) only in Development — mirrors the IsDevelopment()
// guard around Scalar in the other services; the /graphql endpoint itself is always live.
app.MapGraphQL()
    .WithOptions(options => options.Tool.Enable = app.Environment.IsDevelopment());

app.Run();

// Exposed for WebApplicationFactory<Program> in GatewayBff.Tests
public partial class Program;
