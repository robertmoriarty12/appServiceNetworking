# Azure Key Vault Secret Retrieval App

A lightweight ASP.NET Core web application that allows users to retrieve secrets from Azure Key Vault using managed identity authentication.

## Features

- Simple web interface with text input for secret names
- Azure Key Vault integration using managed identity
- Secure secret retrieval without storing credentials
- Clean, responsive UI with Bootstrap

## Prerequisites

- Azure subscription
- Azure Key Vault with secrets stored
- App Service with managed identity enabled
- App Service has Key Vault access permissions

## Deployment Instructions

### 1. Update Configuration

Before deploying, update the `appsettings.json` file with your Key Vault URL:

```json
{
  "KeyVaultUrl": "https://your-keyvault-name.vault.azure.net/"
}
```

### 2. Deploy to Azure App Service

1. **Build the application:**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Create a ZIP file** of the publish folder contents

3. **Deploy via Azure Portal:**
   - Go to your App Service in Azure Portal
   - Navigate to "Deployment Center"
   - Choose "Local Git" or "ZIP Deploy"
   - Upload your ZIP file

4. **Or deploy via Azure CLI:**
   ```bash
   az webapp deployment source config-zip --resource-group <resource-group> --name <app-service-name> --src <path-to-zip>
   ```

### 3. Configure App Service Settings

1. **Enable Managed Identity:**
   - Go to your App Service in Azure Portal
   - Navigate to "Identity" under Settings
   - Turn on "System assigned" identity
   - Note the Object ID

2. **Grant Key Vault Access:**
   - Go to your Key Vault in Azure Portal
   - Navigate to "Access policies" or "Access control (IAM)"
   - Add the App Service's managed identity
   - Grant "Get" permission for secrets

3. **Set Application Settings:**
   - In your App Service, go to "Configuration"
   - Add application setting: `KeyVaultUrl` = `https://your-keyvault-name.vault.azure.net/`

### 4. Test the Application

1. Navigate to your App Service URL
2. Enter a secret name that exists in your Key Vault
3. Click "Retrieve Secret"
4. The secret value should be displayed

## Security Considerations

- The app uses managed identity for authentication (no stored credentials)
- Secrets are retrieved on-demand and not cached
- Ensure proper Key Vault access policies are configured
- Consider using Azure Private Endpoints for enhanced security

## Troubleshooting

- **"Failed to retrieve secret"**: Check that the secret name exists and the App Service has proper permissions
- **Authentication errors**: Verify managed identity is enabled and has Key Vault access
- **Configuration errors**: Ensure KeyVaultUrl is correctly set in app settings

## File Structure

```
KeyVaultSecretApp/
├── Controllers/
│   └── HomeController.cs
├── Models/
│   └── ErrorViewModel.cs
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Error.cshtml
│   ├── Shared/
│   │   └── _Layout.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   └── js/
│       └── site.js
├── KeyVaultSecretApp.csproj
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
└── web.config
```
