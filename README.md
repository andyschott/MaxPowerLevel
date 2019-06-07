# Max Power Level

Max Power Level is a website written with ASP.NET Core 2.2. It shows the highest power gear for your Destiny 2 characters.

When cloning this repository, make sure to clone recursively in order to clone the submodules as well.

# Set up

Download and install the [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download).

In order to run the server, you will need to create an application using the [Bungie Application Portal](https://www.bungie.net/en/Application). When creating the application, do the following:

* Application Name can be whatever you want
* Website can be left blank
* Application Status can be Private
* OAuth Client type should be `Confidential`
* Redirect URL should be `https://localhost:5001/signin-bungie/` (for running in Development mode).
* The only required Scope is `Read your Destiny vault and character inventory.`

When running locally in Development mode, .NET Secrets can be used to store the API keys. Run the following commands from the `MaxPowerLevel` directory:

    dotnet user-secrets set Bugnei:ApiKey <API key>
    dotnet user-secrets set Bungie:ClientId <client id>
    dotnet user-secrets set Bungie:ClientSecret <client secret>

Both the Client ID and Client Secret can be found in the Bungie Application Portal once you have created your application.

# Running

From the `MaxPowerLevel` directory, run `dotnet run` to run the server. The website is now available at `https://localhost:5001`.

# Tests

Unit tests are in the `MaxPowerLevel.Test` project. They can be ran with `dotnet test`.
