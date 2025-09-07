using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
