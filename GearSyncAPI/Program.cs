using GearSyncAPI.Database;
using FastEndpoints;

const string databasePath = "GearSync.db";
DatabaseInitializer.Initialize(databasePath);

var builder = WebApplication.CreateBuilder(args);

// Specify the HTTPS redirection port
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5187); // HTTP
    options.ListenLocalhost(7054, listenOptions => listenOptions.UseHttps());
});


// Add services to the container
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer(); // Register API Explorer
builder.Services.AddSwaggerDocument(); // Enables Swagger for FastEndpoints

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(); // Enable OpenAPI/Swagger UI
    app.UseSwaggerUI(); // Use Swagger UI
}

app.UseHttpsRedirection();

// Use FastEndpoints
app.UseFastEndpoints();

app.Run();