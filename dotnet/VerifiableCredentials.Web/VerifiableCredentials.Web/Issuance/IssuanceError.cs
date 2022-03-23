using Newtonsoft.Json;

namespace VerifiableCredentials.Web;

public class IssuanceError
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}