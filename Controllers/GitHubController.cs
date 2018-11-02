using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Octokit;

namespace GitHubAppDotnetSample.Controllers
{
    public class GitHubController : ControllerBase
    {
        private readonly ILogger<GitHubController> _logger;

        private GitHubClient appClient;

        public GitHubController(IConfiguration configuration, ILogger<GitHubController> logger) 
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

        [GitHubWebHook(EventName = "push", Id = "It")]
        public IActionResult HandlerForItsPushes(string[] events, JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [GitHubWebHook(Id = "It")]
        public IActionResult HandlerForIt(string[] events, JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [GitHubWebHook(EventName = "issues")]
        public async System.Threading.Tasks.Task<IActionResult> HandlerForPushAsync(string id, JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var issueNumber = (int)data["issue"]["number"];
            var installationId = (int)data["installation"]["id"];
            var owner = (string)data["repository"]["owner"]["login"];
            var repo = (string)data["repository"]["name"];

            var response = await appClient.GitHubApps.CreateInstallationToken(installationId);

            var installationClient = new GitHubClient(new ProductHeaderValue($"{Constants.GitHubAppName}_{installationId}"))
            {
                Credentials = new Credentials(response.Token)
            };

            var issueComment = await installationClient.Issue.Comment.Create(owner, repo, issueNumber, "Hello from my GitHubApp Installation!");

            return Ok();
        }

        [GitHubWebHook]
        public IActionResult GitHubHandler(string id, string @event, JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [GeneralWebHook]
        public IActionResult FallbackHandler(string receiverName, string id, string eventName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}