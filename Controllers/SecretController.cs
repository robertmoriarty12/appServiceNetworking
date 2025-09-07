using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;

namespace KeyVaultSecretApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecretController : ControllerBase
    {
        private readonly SecretClient _secretClient;
        private readonly ILogger<SecretController> _logger;

        public SecretController(SecretClient secretClient, ILogger<SecretController> logger)
        {
            _secretClient = secretClient;
            _logger = logger;
        }

        [HttpGet("{secretName}")]
        public async Task<IActionResult> GetSecret(string secretName)
        {
            try
            {
                _logger.LogInformation("Retrieving secret: {SecretName}", secretName);

                var secret = await _secretClient.GetSecretAsync(secretName);
                
                _logger.LogInformation("Successfully retrieved secret: {SecretName}", secretName);

                return Ok(new
                {
                    name = secretName,
                    value = secret.Value.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve secret: {SecretName}", secretName);
                return StatusCode(500, new
                {
                    error = "Failed to retrieve secret",
                    message = ex.Message,
                    secretName = secretName,
                    keyVaultUrl = _secretClient.VaultUrl,
                    exceptionType = ex.GetType().Name,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}
