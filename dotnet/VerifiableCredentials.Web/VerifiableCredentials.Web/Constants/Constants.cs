namespace VerifiableCredentials.Web;

public static class Constants
{
    internal const string VerifiableCredentials = "VerifiableCredentials";

    internal const string Endpoint = "https://beta.did.msidentity.com/v1.0/{0}/verifiablecredentials/request";

    internal const string VerifiableCredentialsServiceScope = "3db474b9-6a0c-4840-96ac-1fceb342124f/.default";

    internal const string Instance = "https://login.microsoftonline.com/{0}";

    internal const string Manifest = "https://beta.did.msidentity.com/v1.0/{0}/verifiableCredential/contracts/{1}";

    internal const string IssuancePath = "verifiablecredentials/issuance";

    internal const string PresentationPath = "verifiablecredentials/presentation";
}