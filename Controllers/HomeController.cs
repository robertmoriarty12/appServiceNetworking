using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;

namespace KeyVaultSecretApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly SecretClient _secretClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(SecretClient secretClient, ILogger<HomeController> logger)
        {
            _secretClient = secretClient;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetSecret(string secretName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(secretName))
                {
                    ViewBag.Error = "Please enter a message";
                    return View("Index");
                }

                // Limit to 1000 characters
                if (secretName.Length > 1000)
                {
                    ViewBag.Error = "Message cannot exceed 1000 characters";
                    return View("Index");
                }

                // Check if the message is exactly "secret"
                if (secretName.Trim().ToLower() == "secret")
                {
                    _logger.LogInformation("Secret keyword detected, attempting to retrieve secret from Key Vault");

                    // For now, we'll use a default secret name - you can change this
                    var defaultSecretName = "my-secret"; // Change this to your actual secret name
                    
                    var secret = await _secretClient.GetSecretAsync(defaultSecretName);
                    ViewBag.SecretValue = secret.Value.Value;
                    ViewBag.SecretName = defaultSecretName;
                    ViewBag.Success = true;
                    ViewBag.IsSecret = true;

                    _logger.LogInformation("Successfully retrieved secret: {SecretName}", defaultSecretName);
                }
                else
                {
                    // Just return the message as-is
                    ViewBag.Message = secretName;
                    ViewBag.Success = true;
                    ViewBag.IsSecret = false;
                    
                    _logger.LogInformation("Message received: {Message}", secretName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process request: {Message}", secretName);
                ViewBag.Error = $"Failed to process request: {ex.Message}";
            }

            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
