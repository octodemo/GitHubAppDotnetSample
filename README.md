# GitHubAppDotnetSample

[![Build Status](https://dev.azure.com/octodemo-readingtimedemo/GitHubAppDotnetSample/_apis/build/status/octodemo.GitHubAppDotnetSample)](https://dev.azure.com/octodemo-readingtimedemo/GitHubAppDotnetSample/_build/latest?definitionId=5) [![Release](https://vsrm.dev.azure.com/octodemo-readingtimedemo/_apis/public/Release/badge/c759e395-e69d-415c-b45a-7087d8b65f3e/1/1)](https://dev.azure.com/octodemo-readingtimedemo/GitHubAppDotnetSample/)

A sample GitHub App written in .NET Core with Azure Pipelines for build and deploy.

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

- [Register a GitHub App](https://developer.github.com/apps/building-your-first-github-app/#register-a-new-app-with-github). The sample requires Read & Write access for Issues and a subscription to Issues events. When testing the smee URL (like https://smee.io/qrfeVRbFbffd6vD) should be used for the URL fields.
- Download the private key and you also need the Application ID.
- Add the following configuration settings in an `appsettings.json` like for example `apsettings.Development.json`. The Webhook secret needs to be at least 16 characters.

```json
{
  "WebHooks:GitHub:SecretKey:default": "0123456789012345",
  "GitHubApp:ApplicationId": 19892,
  "GitHubApp:PrivateKey:file": "path/to/your/private-key.pem"

}
```

- Start the `GitHubAppDotnetSample` GitHub App

```
dotnet run
```

## Private key settings

There are three ways to add the private key:

- `GitHubApp:PrivateKey:file`
  - As a local path to the private key file
- `GitHubApp:PrivateKey:string`
  - As a string value. You can replace new lines using: `awk '{printf "%s\\n", $0}' path/to/your/private-key.pem`
- `GitHubApp:PrivateKey:base64`
  - As a Base64 encoded string using fo example: `cat path/to/your/private-key.pem | openssl base64 | pbcopy`


