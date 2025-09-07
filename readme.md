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

## Customer Deployment Instructions

### Prerequisites
- Azure subscription
- Azure CLI installed (for Cloud Shell deployment)

### Step 1: Deploy Azure Resources

1. **Create App Service:**
   - Runtime Stack: `Node.js 22 LTS`
   - Platform: `64 Bit`
   - Note your App Service name and Resource Group

2. **Create Azure Key Vault:**
   - Note your Key Vault URL (e.g., `https://your-keyvault-name.vault.azure.net`)

3. **Create Secret in Key Vault:**
   - Name: `my-secret`
   - Value: `Hello, this is the secret from key vault`

### Step 2: Configure App Service

1. **Enable Managed Identity:**
   - Go to App Service → Identity → System assigned → Turn ON
   - Copy the Object ID

2. **Grant Key Vault Access:**
   - Go to Key Vault → Access policies (or Access control IAM)
   - Add App Service's managed identity
   - Grant Key Vault admin permissions over the resource group

3. **Set App Setting:**
   - Go to App Service → Configuration → Application settings
   - Add: `KeyVaultUrl` = `https://your-keyvault-name.vault.azure.net` (no trailing slash)

### Step 3: Deploy Application

1. **Upload ZIP to Cloud Shell:**
   - Open Azure Cloud Shell
   - Upload `KeyVaultSecretApp-Working.zip`

2. **Deploy via Azure CLI:**
   ```bash
   az webapp deploy --resource-group "your-resource-group" --name "your-app-service-name" --src-path "KeyVaultSecretApp-Working.zip" --type zip
   ```

3. **Test your app:**
   - Visit: `https://your-app-service-name.azurewebsites.net`
   - Type any message → should echo back
   - Type `secret` → should retrieve actual secret from Key Vault

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
