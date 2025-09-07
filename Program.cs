using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Key Vault client with managed identity
builder.Services.AddSingleton<SecretClient>(provider =>
{
    var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
    if (string.IsNullOrEmpty(keyVaultUrl))
    {
        throw new InvalidOperationException("KeyVaultUrl configuration is required");
    }
    
    return new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

// Serve the HTML file
app.MapGet("/", () => Results.Content(System.IO.File.ReadAllText("index.html"), "text/html"));

app.Run();