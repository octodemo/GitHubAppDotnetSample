using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;

namespace GitHubAppDotnetSample.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> _logger;

        protected GitHubClient appClient;

        protected BaseController(IConfiguration configuration, ILogger<GitHubController> logger)
        {
            _logger = logger;

            var appIdPath = ConfigurationPath.Combine(Constants.GitHubAppSectionKey, Constants.ApplicationId);

            var privateKeyPath = ConfigurationPath.Combine(Constants.GitHubAppSectionKey, Constants.PrivateKey);

            var appId = configuration.GetSection(appIdPath);

            var privateKey = configuration.GetSection(privateKeyPath);

            var generator = new GitHubJwt.GitHubJwtFactory(
                new GitHubJwt.FilePrivateKeySource(privateKey.Value),
                    new GitHubJwt.GitHubJwtFactoryOptions
                    {
                        AppIntegrationId = int.Parse(appId.Value), // The GitHub App Id
                        ExpirationSeconds = 600 // 10 minutes is the maximum time allowed
                    }
            );

            var jwtToken = generator.CreateEncodedJwtToken();

            // Pass the JWT as a Bearer token to Octokit.net
            appClient = new GitHubClient(new ProductHeaderValue(Constants.GitHubAppName))
            {
                Credentials = new Credentials(jwtToken, AuthenticationType.Bearer)
            };

        }
    }
}
