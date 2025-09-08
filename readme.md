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

### Step 4: Application Gateway + WAF Configuration (Optional)

For production deployments with Application Gateway and WAF protection:

#### Prerequisites
- Virtual Network with Application Gateway subnet
- Custom domain names (e.g., `yourapp1.com`, `yourapp2.com`)
- SSL certificates for custom domains

#### 4.1 Deploy Application Gateway

1. **Use Gateway-Ready App:**
   - Deploy `KeyVaultSecretApp-WithGateway.zip` to your App Services
   - This version includes `app.set('trust proxy', true)` for proper proxy handling

2. **Create Application Gateway:**
   ```bash
   # Create Application Gateway with WAF v2
   az network application-gateway create \
     --resource-group "your-resource-group" \
     --name "your-app-gateway" \
     --location "centralus" \
     --sku WAF_v2 \
     --capacity 2 \
     --vnet-name "your-vnet" \
     --subnet "ApplicationGatewaySubnet" \
     --public-ip-address "your-public-ip"
   ```

#### 4.2 Configure DNS Zones

1. **Create DNS Zones:**
   ```bash
   # Create DNS zones for custom domains
   az network dns zone create \
     --resource-group "your-resource-group" \
     --name "yourapp1.com"
   
   az network dns zone create \
     --resource-group "your-resource-group" \
     --name "yourapp2.com"
   ```

2. **Add A Records:**
   ```bash
   # Point domains to Application Gateway public IP
   az network dns record-set a add-record \
     --resource-group "your-resource-group" \
     --zone-name "yourapp1.com" \
     --record-set-name "@" \
     --ipv4-address "your-app-gateway-public-ip"
   ```

#### 4.3 Configure SSL Certificates

1. **Upload SSL Certificates:**
   ```bash
   # Upload certificate for app1
   az network application-gateway ssl-cert create \
     --resource-group "your-resource-group" \
     --gateway-name "your-app-gateway" \
     --name "app1cert" \
     --cert-file "app1.crt" \
     --cert-password "your-password"
   ```

#### 4.4 Configure Backend Pools

1. **Create Backend Pools:**
   ```bash
   # Backend pool for app1
   az network application-gateway address-pool create \
     --resource-group "your-resource-group" \
     --gateway-name "your-app-gateway" \
     --name "app1" \
     --servers "yourapp1.azurewebsites.net"
   
   # Backend pool for app2
   az network application-gateway address-pool create \
     --resource-group "your-resource-group" \
     --gateway-name "your-app-gateway" \
     --name "app2" \
     --servers "yourapp2.azurewebsites.net"
   ```

#### 4.5 Configure Health Probes

1. **Create Health Probes:**
   ```bash
   # Health probe for app1
   az network application-gateway probe create \
     --resource-group "your-resource-group" \
     --gateway-name "your-app-gateway" \
     --name "probeapp1" \
     --protocol Https \
     --host "yourapp1.azurewebsites.net" \
     --path "/health" \
     --interval 30 \
     --timeout 30 \
     --threshold 3
   ```

#### 4.6 Configure HTTP Settings

1. **Create Backend HTTP Settings:**
   ```bash
   # HTTP settings for app1
   az network application-gateway http-settings create \
     --resource-group "your-resource-group" \
     --gateway-name "your-app-gateway" \
     --name "app1backend" \
     --port 443 \
     --protocol Https \
     --host-name "yourapp1.azurewebsites.net" \
     --probe "probeapp1" \
     --timeout 20
   ```

#### 4.7 Configure Listeners

1. **Create HTTPS Listeners:**
   ```bash
   # Listener for app1
   az network application-gateway http-listener create \
     --resource-group "your-resource-group" \
     --gateway-name "your-app-gateway" \
     --name "app1listener" \
     --frontend-port "port_443" \
     --frontend-ip "appGwPublicFrontendIpIPv4" \
     --host-name "yourapp1.com" \
     --ssl-cert "app1cert"
   ```

#### 4.8 Configure Routing Rules

1. **Create Routing Rules:**
   ```bash
   # Routing rule for app1
   az network application-gateway rule create \
     --resource-group "your-resource-group" \
     --gateway-name "your-app-gateway" \
     --name "routerule1" \
     --rule-type Basic \
     --http-listener "app1listener" \
     --address-pool "app1" \
     --http-settings "app1backend" \
     --priority 1
   ```

#### 4.9 Configure WAF Policy

1. **Create WAF Policy:**
   ```bash
   # Create WAF policy with OWASP 3.2 rules
   az network application-gateway waf-policy create \
     --resource-group "your-resource-group" \
     --name "wafpolicy" \
     --location "centralus"
   
   # Add OWASP rule set
   az network application-gateway waf-policy managed-rule-set add \
     --resource-group "your-resource-group" \
     --policy-name "wafpolicy" \
     --rule-set-type OWASP \
     --rule-set-version 3.2
   
   # Set WAF mode to Prevention
   az network application-gateway waf-policy policy-setting update \
     --resource-group "your-resource-group" \
     --policy-name "wafpolicy" \
     --state Enabled \
     --mode Prevention
   ```

2. **Attach WAF Policy to Application Gateway:**
   ```bash
   az network application-gateway update \
     --resource-group "your-resource-group" \
     --name "your-app-gateway" \
     --set firewallPolicy.id="/subscriptions/your-subscription/resourceGroups/your-resource-group/providers/Microsoft.Network/ApplicationGatewayWebApplicationFirewallPolicies/wafpolicy"
   ```

#### 4.10 Test WAF Protection

1. **Test Path Traversal Attack:**
   ```bash
   curl -k -i "https://yourapp1.com/?file=../../../../etc/passwd"
   # Should return 403 Forbidden (WAF blocked)
   ```

2. **Test SQL Injection:**
   ```bash
   curl -k -i "https://yourapp1.com/api/secret/my-secret' OR 1=1--"
   # Should return 403 Forbidden (WAF blocked)
   ```

3. **Test Normal Traffic:**
   ```bash
   curl -k -i "https://yourapp1.com/"
   # Should return 200 OK (normal traffic allowed)
   ```

#### 4.11 Example Configuration (Based on Working Setup)

**Your Working Configuration:**
- **Application Gateway**: `secretAppWafPoCAppGateway`
- **Resource Group**: `secretAppWafPoC`
- **Custom Domains**: 
  - `secretappwafpoc1.com` → `secretappwafpocapp1-euh9bkgjf2d6acea.centralus-01.azurewebsites.net`
  - `secretappwafpoc2.com` → `secretappwafpocapp2-aaf8hddmadftd7h4.centralus-01.azurewebsites.net`
- **WAF Policy**: `wafpolicy` (OWASP 3.2, Prevention mode)
- **Backend Pools**: `app1`, `app2`
- **Health Probes**: `/` path, HTTPS, 30s interval
- **SSL Certificates**: Self-signed certificates for custom domains

**Key Configuration Details:**
- **SKU**: WAF_v2 (Generation 2)
- **Capacity**: 2 instances
- **Zones**: 1, 2, 3 (multi-zone deployment)
- **Backend Protocol**: HTTPS (port 443)
- **Request Timeout**: 20 seconds
- **Health Probe**: HTTPS to `/` endpoint

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

### Debugging Commands

**View real-time logs:**
```bash
az webapp log tail --resource-group "your-resource-group" --name "your-app-service-name"
```

**Download logs for analysis:**
```bash
az webapp log download --resource-group "your-resource-group" --name "your-app-service-name" --log-file "logs.zip"
```

**Check App Service configuration:**
```bash
az webapp config show --resource-group "your-resource-group" --name "your-app-service-name" --query "linuxFxVersion"
```

**Test API endpoint directly:**
```bash
curl https://your-app-service-name.azurewebsites.net/api/secret/my-secret
```

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
