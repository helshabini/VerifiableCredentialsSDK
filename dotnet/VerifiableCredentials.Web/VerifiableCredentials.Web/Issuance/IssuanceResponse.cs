using Newtonsoft.Json;

namespace VerifiableCredentials.Web.Issuance;

public class IssuanceResponse
{
    [JsonProperty("requestId")]
    [JsonRequired]
    public Guid RequestId { get; set; }

    [JsonProperty("url", NullValueHandling=NullValueHandling.Ignore)] 
    public string Url { get; set; } = null!;

    [JsonProperty("expiry", NullValueHandling=NullValueHandling.Ignore)] 
    public int Expiry { get; set; }

    [JsonProperty("qrCode", NullValueHandling=NullValueHandling.Ignore)] 
    public string QrCode { get; set; } = null!;

    [JsonProperty("date", NullValueHandling=NullValueHandling.Ignore)]
    public DateTimeOffset Date { get; set; }

    [JsonProperty("error", NullValueHandling=NullValueHandling.Ignore)]
    public IssuanceError Error { get; set; } = null!;
    
    public static IssuanceResponse? FromJson(string json) =>
        JsonConvert.DeserializeObject<IssuanceResponse>(json, IssuanceJsonConverter.Settings);

    public string ToJson() =>
        JsonConvert.SerializeObject(this, IssuanceJsonConverter.Settings);
}