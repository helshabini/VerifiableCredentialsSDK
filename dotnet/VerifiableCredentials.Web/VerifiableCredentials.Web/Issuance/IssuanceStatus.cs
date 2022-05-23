using Newtonsoft.Json;

namespace VerifiableCredentials.Web.Issuance;

public class IssuanceStatus
{
    [JsonProperty("request", NullValueHandling=NullValueHandling.Ignore)]
    public IssuanceRequest? Request { get; set; }
    
    [JsonProperty("response", NullValueHandling=NullValueHandling.Ignore)]
    public IssuanceResponse? Response { get; set; }
    
    [JsonProperty("callback", NullValueHandling=NullValueHandling.Ignore)]
    public IssuanceCallback? Callback { get; set; }
    
    public static IssuanceStatus? FromJson(string json) =>
        JsonConvert.DeserializeObject<IssuanceStatus>(json, IssuanceJsonConverter.Settings);

    public string ToJson() =>
        JsonConvert.SerializeObject(this, IssuanceJsonConverter.Settings);
}