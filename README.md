# Azure AD Verifiable Credentials SDK for DotNet 6.0

This is an SDK built to simplify issuing and presenting verifiable credentials for Azure AD Verifiable Credentials. This is a work in progress, any contribution is appreciated.

## License
This software is not warrantied, sponsored, affiliated, or guaranteed by Microsoft in any way.
See the [LICENSE](LICENSE.md) file for license rights and limitations (MIT).

## Usage

In your `appsettings.json`

```json
"VCIssuanceOptions": [
    {
        "CredentialType": "VerifiedCredentialExpert",
        "Authority": "YOUR-DID-ION",
        "ClientName": "verifiable-credentials-app",
        "TenantId": "YOUR-TENANT-ID",
        "ClientId": "YOUR-CLIENT-ID",
        "ClientSecret": "YOUR-CLIENT-SECRET"
    },
    {
        "CredentialType": "AnotherVerifiedCredential",
        "Authority": "YOUR-DID-ION",
        "ClientName": "verifiable-credentials-app",
        "TenantId": "YOUR-TENANT-ID",
        "ClientId": "YOUR-CLIENT-ID",
        "ClientSecret": "YOUR-CLIENT-SECRET"
    }
]
```

In your `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Verifiable Credentials
builder.Services.AddVerifiableCredentials()
    .WithIssuanceRequests(config: builder.Configuration.GetSection("VCIssuanceOptions"));

...

var app = builder.Build();

...

app.UseVerifiableCredentials()
```

The SDK uses a custom middleware to inject the following endpoints into your application:

| Endpoint                                                  | Method | Parameters                                                           | Response        | Description                                                                                                                                                      |
|-----------------------------------------------------------|--------|----------------------------------------------------------------------|-----------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| /verifiablecredentials/issuance/request/{credential name} | POST   | - (Body/Optional) Claims in json format                              | Issuance Status | You can use this endpoint to issue a credential by name                                                                                                          |
| /verifiablecredentials/issuance/status?id={request id}    | GET    | - (QueryString/Required) Request Id                                  | Issuance Status | You can use this endpoint to check the status of an issuance request                                                                                             |
| /verifiablecredentials/issuance/callback                  | GET    | - (Header/Optional) ApiKey - (Body/Required) Callback in json format | None            | This endpoint is used by the Verifiable Credentials Service to notify the SDK of an update in a request's status. You should not use this endpoint in your code. |

You can also use the SDK's Verifiable Credentials Service via Dependency Injection to issue requests from code rather than using the geenrated endpoints.

```csharp
using VerifiableCredentials.Web;

public class MyClass
{
    private readonly IVerifiableCredentialsService _vcService;

    public MyClass(IVerifiableCredentialsService vcService)
    {
        _vcService = vcService;
    }

    public void MyMethod()
    {
        //Issuing a credential without claims
        var status = _vcService.IssueCredentialAsync("VerifiedCredentialExpert", new Uri("https://myapp.com"));
    
        //Issuing a credential with claims
        var claims = new Dictionary<string, string>();
        claims.Add("given_name", "Megan");
        claims.Add("family_name", "Bowen");
        var status = _vcService.IssueCredentialAsync("VerifiedCredentialExpert", new Uri("https://myapp.com"), claims);
        
        //Checking status of an issuance request
        var status = _vcService.RequestStatusAsync("request id");
    }
}
```

## Upcoming updates

- Currently, the SDK does issuance only. Credential verification shall be added in the near future.
- I'm also working on the ability to bubble up callback events somehow so that your app is also notified when an update has arrived. Currently, the only way to know if an update has arrived is to keep polling the status endpoint or call the RequestStatusAsync method.
- I'm also working on adding a `Build()` method to the VerifiableCredentialsBuilder to be able to manually create your own VerifiableCredentialService instances and use it without having to use DependencyInjection.
- Nuget package will created and published in the near future