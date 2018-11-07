using System;
using System.Configuration;
using System.Text;
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

            var jwtToken = getJwtToken(configuration);

            // Pass the JWT as a Bearer token to Octokit.net
            appClient = new GitHubClient(new ProductHeaderValue(Constants.GitHubAppName))
            {
                Credentials = new Credentials(jwtToken, AuthenticationType.Bearer)
            };

        }

        private string getJwtToken(IConfiguration configuration) {

            var appIdPath = ConfigurationPath.Combine(Constants.GitHubAppSectionKey, Constants.ApplicationId);

            var privateKeyString = ConfigurationPath.Combine(Constants.GitHubAppSectionKey, Constants.PrivateKey, Constants.PrivateKeyString);

            var privateKeyBase64 = ConfigurationPath.Combine(Constants.GitHubAppSectionKey, Constants.PrivateKey, Constants.PrivateKeyBase64);

            var privateKeyFile = ConfigurationPath.Combine(Constants.GitHubAppSectionKey, Constants.PrivateKey, Constants.PrivateKeyFile);

            var appId = configuration.GetSection(appIdPath);

            if (configuration.GetSection(privateKeyString).Exists())
            {

                _logger.LogInformation("Using private key string value");

                var privateKey = configuration.GetSection(privateKeyString).Value.Replace("\n",Environment.NewLine);

                var generator = new GitHubJwt.GitHubJwtFactory(
                new GitHubJwt.StringPrivateKeySource(privateKey),
                    new GitHubJwt.GitHubJwtFactoryOptions
                    {
                        AppIntegrationId = int.Parse(appId.Value), // The GitHub App Id
                        ExpirationSeconds = 600 // 10 minutes is the maximum time allowed
                    }
                );

                return generator.CreateEncodedJwtToken();
            }
            else if (configuration.GetSection(privateKeyBase64).Exists())
            {

                _logger.LogInformation("Using private key base64 encoded value");

                var encodedKey = configuration.GetSection(privateKeyBase64);

                byte[] data = Convert.FromBase64String(encodedKey.Value);
                string decodedString = Encoding.UTF8.GetString(data);

                var generator = new GitHubJwt.GitHubJwtFactory(
                new GitHubJwt.StringPrivateKeySource(decodedString),
                    new GitHubJwt.GitHubJwtFactoryOptions
                    {
                        AppIntegrationId = int.Parse(appId.Value), // The GitHub App Id
                        ExpirationSeconds = 600 // 10 minutes is the maximum time allowed
                    }
                );

                return generator.CreateEncodedJwtToken();

            }
            else if (configuration.GetSection(privateKeyFile).Exists())
            {
                _logger.LogInformation("Using private key file reference");

                var privateKey = configuration.GetSection(privateKeyFile);

                var generator = new GitHubJwt.GitHubJwtFactory(
                new GitHubJwt.FilePrivateKeySource(privateKey.Value),
                    new GitHubJwt.GitHubJwtFactoryOptions
                    {
                        AppIntegrationId = int.Parse(appId.Value), // The GitHub App Id
                        ExpirationSeconds = 600 // 10 minutes is the maximum time allowed
                    }
                );

                return generator.CreateEncodedJwtToken();
            }
            else
            {
                _logger.LogInformation("No private key setting configured");

                return null;
            }

        }
    }
}
