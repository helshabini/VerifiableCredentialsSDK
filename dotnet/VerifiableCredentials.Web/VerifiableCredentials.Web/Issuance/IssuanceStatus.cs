using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VerifiableCredentials.Web.Issuance;

public class IssuanceStatus
{
    [JsonProperty("requestId")]
    [JsonRequired]
    public Guid RequestId { get; set; }
    
    [JsonProperty("url", NullValueHandling=NullValueHandling.Ignore)] 
    public string Url { get; set; } = null!;
    
    [JsonProperty("qrCode", NullValueHandling=NullValueHandling.Ignore)] 
    public string? QrCode { get; set; }
    
    [JsonProperty("pin", NullValueHandling=NullValueHandling.Ignore)] 
    public string? Pin { get; set; }
    
    [JsonProperty("apiKey", NullValueHandling=NullValueHandling.Ignore)] 
    public string? ApiKey { get; set; }
    
    [JsonProperty("code", NullValueHandling=NullValueHandling.Ignore)]
    [JsonConverter(typeof(StringEnumConverter))]
    public CallbackCode? Code { get; set; }
    
    [JsonProperty("error", NullValueHandling=NullValueHandling.Ignore)]
    public IssuanceError? Error { get; set; }
    
    public static IssuanceStatus? FromJson(string json) =>
        JsonConvert.DeserializeObject<IssuanceStatus>(json, IssuanceJsonConverter.Settings);

    public string ToJson() =>
        JsonConvert.SerializeObject(this, IssuanceJsonConverter.Settings);
}