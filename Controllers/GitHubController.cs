using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Octokit;

namespace GitHubAppDotnetSample.Controllers
{
    public class GitHubController : BaseController
    {
        public GitHubController(IConfiguration configuration, ILogger<GitHubController> logger) : base(configuration, logger) {

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