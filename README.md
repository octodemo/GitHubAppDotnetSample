# GitHubAppDotnetSample

A sample GitHub App written in .NET Core.

The boilerplate code for this example is based on [GitHubCoreReceiver](https://github.com/aspnet/AspLabs/tree/master/src/WebHooks/samples/GitHubCoreReceiver) with JWT token authentication based on [Working with GitHub Apps](https://octokitnet.readthedocs.io/en/latest/github-apps/) in the Octokit.net documentation.

## Installation

To test locally:

- Clone the repository.
- Go to https://smee.io and click Start a new channel.
- Install the client:

```
npm install --global smee-client
```

- Run the client (replacing https://smee.io/qrfeVRbFbffd6vD with your own domain):

```
smee -u https://smee.io/qrfeVRbFbffd6vD -P /api/webhooks/incoming/github
```

- You should see output like the following:

```
Forwarding https://smee.io/qrfeVRbFbffd6vD to http://127.0.0.1:3000/api/webhooks/incoming/github
Connected https://smee.io/qrfeVRbFbffd6vD
```

- [Register a GitHub App](https://developer.github.com/apps/building-your-first-github-app/#register-a-new-app-with-github)
- Add the following configuration settings in an environment specific `appsettings.json` like for example `apsettings.Development.json`. The Webhook secret mneeds to be at least 16 characters in length.

```json
{
  "WebHooks:GitHub:SecretKey:default": "0123456789012345",
  "GitHubApp:ApplicationId": 19892,
  "GitHubApp:PrivateKey.file": "/path/to/private_key.pem"
}
```

- Start the `GitHubAppDotnetSample` GitHub App

```
dotnet run
```
