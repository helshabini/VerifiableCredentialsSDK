using Newtonsoft.Json;

namespace VerifiableCredentials.Web.Issuance;

public class IssuanceError
{
    [JsonProperty("code", NullValueHandling=NullValueHandling.Ignore)]
    public string Code { get; set; } = null!;

    [JsonProperty("message", NullValueHandling=NullValueHandling.Ignore)]
    public string Message { get; set; } = null!;
}