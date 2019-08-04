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

    dotnet user-secrets set Bungie:ApiKey <API key>
    dotnet user-secrets set Bungie:ClientId <client id>
    dotnet user-secrets set Bungie:ClientSecret <client secret>

The API key, Client ID and Client Secret can be found in the Bungie Application Portal once you have created your application.

# Running

Bungie requires using HTTPS when using their API. If you have not done so already, you need to trust the ASP.NET Core development certificate:

    dotnet dev-certs https --trust

## Local

From the `MaxPowerLevel` directory, run `dotnet run` to run the server. The website is now available at `https://localhost:5001`.

## Docker

In order to get HTTPS working when running in Docker, you must share your development certificate with the .NET Core runtime in the Docker image. Perform the following steps in order to do that:

1. Create a directory named `cert` in the root of the repository.
2. Run the following command:  
`dotnet dev-certs https -ep cert/cert-aspnetcore.pfx -p <password>`

Replace `<password>` with a password of your choosing.

To run in Docker, create a file named `environment.txt` in the root directory. Insert the following text into it:

    ASPNETCORE_Bungie:ClientSecret=<client secret>
    ASPNETCORE_Bungie:ClientId=<client id>
    ASPNETCORE_Bungie:ApiKey=<API key>
    ASPNETCORE_HTTPS_PORT=5001
    ASPNETCORE_URLS=https://+;http://+
    Kestrel__Certificates__Default__Path=/.dotnet/https/cert-aspnetcore.pfx
    Kestrel__Certificates__Default__Password=<password>

Set the client secret, client ID, and API key to the correct values. Set `<password>` to the same password used when generating the development certificate above.

Start a new image containing this application by running `docker-compose up` from the root folder. Once it has finished building and running the image, the website will be available at `https://localhost:5001`.

Note that `docker-compose` does not automatically rebuild the image when changes occur to the source code. After you have started the image once, if you make any changes to the source code you will need to rebuild the image by running `docker-compose build`.

# Tests

Unit tests are in the `MaxPowerLevel.Test` project. They can be ran with `dotnet test`.
