const express = require('express');
const { DefaultAzureCredential } = require('@azure/identity');
const { SecretClient } = require('@azure/keyvault-secrets');

const app = express();
const port = process.env.PORT || 8080;

// Trust proxy for Application Gateway
app.set('trust proxy', true);

// Middleware
app.use(express.json());
app.use(express.static('.'));

// Get Key Vault URL from environment variable
const keyVaultUrl = process.env.KeyVaultUrl || 'https://akvaspnetworking.vault.azure.net/';

// Initialize Key Vault client with managed identity
const credential = new DefaultAzureCredential();
const client = new SecretClient(keyVaultUrl, credential);

// API endpoint to retrieve secret
app.get('/api/secret/:secretName', async (req, res) => {
    try {
        const secretName = req.params.secretName;
        console.log(`Retrieving secret: ${secretName}`);
        console.log(`Key Vault URL: ${keyVaultUrl}`);
        console.log(`Request from IP: ${req.ip}`);
        console.log(`X-Forwarded-For: ${req.get('X-Forwarded-For')}`);
        console.log(`X-Forwarded-Proto: ${req.get('X-Forwarded-Proto')}`);
        
        const secret = await client.getSecret(secretName);
        
        console.log(`Successfully retrieved secret: ${secretName}`);
        
        res.json({
            name: secretName,
            value: secret.value
        });
    } catch (error) {
        console.error('Error retrieving secret:', error);
        res.status(500).json({
            error: 'Failed to retrieve secret',
            message: error.message,
            secretName: req.params.secretName,
            keyVaultUrl: keyVaultUrl,
            exceptionType: error.constructor.name,
            stackTrace: error.stack
        });
    }
});

// Health check endpoint
app.get('/health', (req, res) => {
    res.json({ status: 'OK', timestamp: new Date().toISOString() });
});

app.listen(port, () => {
    console.log(`Server running on port ${port}`);
    console.log(`Key Vault URL: ${keyVaultUrl}`);
});