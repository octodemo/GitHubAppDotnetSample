using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using GitHubJwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;

namespace GitHubAppDotnetSample.Filters
{
    public class CreateGitHubClientFilter : IAsyncResourceFilter
    {

        readonly IConfiguration _configuration;

        public CreateGitHubClientFilter(IConfiguration configuration) => _configuration = configuration;

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var jwtToken = getJwtToken(_configuration);

            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new NullReferenceException("Unable to generate token");
            }

            // Pass the JWT as a Bearer token to Octokit.net
            GitHubClient appClient = new GitHubClient(new ProductHeaderValue(Constants.GitHubAppName))
            {
                Credentials = new Credentials(jwtToken, AuthenticationType.Bearer)
            };

            context.RouteData.Values[Constants.GitHubClient] = appClient;

            await next();
        }

        private string getJwtToken(IConfiguration configuration)
        {

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var appIdPath = ConfigurationPath.Combine(Constants.GitHubAppSectionKey, Constants.ApplicationId);

            var privateKeyString = ConfigurationPath.Combine(Constants.GitHubAppSectionKey, Constants.PrivateKey, Constants.PrivateKeyString);

            var privateKeyBae64 = ConfigurationPath.Combine(Constants.GitHubAppSectionKey, Constants.PrivateKey, Constants.PrivateKeyBase64);

            var privateKeyFile = ConfigurationPath.Combine(Constants.GitHubAppSectionKey, Constants.PrivateKey, Constants.PrivateKeyFile);

            var appId = configuration.GetSection(appIdPath);

            var options = new GitHubJwt.GitHubJwtFactoryOptions
            {
                AppIntegrationId = int.Parse(appId.Value), // The GitHub App Id
                ExpirationSeconds = 600 // 10 minutes is the maximum time allowed
            };

            if (configuration.GetSection(privateKeyString).Exists())
            {
                // use `awk '{printf "%s\\n", $0}' private-key.pem` to store the pem data
                var privateKey = configuration.GetSection(privateKeyString).Value.Replace("\n", Environment.NewLine);

                var generator = new GitHubJwtFactory(
                new StringPrivateKeySource(privateKey), options);

                return generator.CreateEncodedJwtToken();
            }
            else if (configuration.GetSection(privateKeyBae64).Exists())
            {
                var encodedKey = configuration.GetSection(privateKeyBae64);

                byte[] data = Convert.FromBase64String(encodedKey.Value);
                string decodedString = Encoding.UTF8.GetString(data);

                var generator = new GitHubJwtFactory(
                new StringPrivateKeySource(decodedString), options);

                return generator.CreateEncodedJwtToken();

            }
            else if (configuration.GetSection(privateKeyFile).Exists())
            {
                var privateKey = configuration.GetSection(privateKeyFile);

                var generator = new GitHubJwtFactory(
                new FilePrivateKeySource(privateKey.Value), options);

                return generator.CreateEncodedJwtToken();
            }
            else
            {
                return null;
            }

        }
    }
}