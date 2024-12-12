using GearSyncAPI.Database;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

const string databasePath = "GearSync.db";
DatabaseInitializer.Initialize(databasePath);

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on specific ports
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5187); // HTTP
    options.ListenLocalhost(7054, listenOptions => listenOptions.UseHttps()); // HTTPS
});

// Add services to the container
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer(); // Register API Explorer
builder.Services.AddSwaggerDocument(); // Enables Swagger for FastEndpoints

// Configure CORS to allow specific origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
        policy.WithOrigins("http://localhost:5173") // Specify the allowed origin
            .AllowAnyMethod() // Allows all methods including OPTIONS
            .AllowAnyHeader() // Allows all headers
            .AllowCredentials()); // Allows credentials like cookies, authorization headers etc.
});

// Register the background service to clean expired sessions
builder.Services.AddHostedService<SessionCleanupService>(); // Register your background service here

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(); // Enable OpenAPI/Swagger UI
    app.UseSwaggerUI(); // Use Swagger UI
    // app.UseHttpsRedirection(); // Temporarily disable to test CORS
}

// Apply CORS policy
app.UseCors("AllowSpecificOrigin");

// Use FastEndpoints
app.UseFastEndpoints();

app.Run();