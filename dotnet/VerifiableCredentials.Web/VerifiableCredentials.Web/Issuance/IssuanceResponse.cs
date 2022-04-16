using Newtonsoft.Json;

namespace VerifiableCredentials.Web.Issuance;

public class IssuanceResponse
{
    [JsonProperty("requestId")]
    [JsonRequired]
    public Guid RequestId { get; set; }

    [JsonProperty("url", NullValueHandling=NullValueHandling.Ignore)] 
    public string Url { get; set; }

    [JsonProperty("expiry", NullValueHandling=NullValueHandling.Ignore)] 
    public int Expiry { get; set; }

    [JsonProperty("qrCode", NullValueHandling=NullValueHandling.Ignore)] 
    public string QrCode { get; set; }

    [JsonProperty("date", NullValueHandling=NullValueHandling.Ignore)]
    public string Date { get; set; }

    [JsonProperty("error", NullValueHandling=NullValueHandling.Ignore)]
    public Error Error { get; set; }
    
    public static IssuanceResponse? FromJson(string json) =>
        JsonConvert.DeserializeObject<IssuanceResponse>(json, IssuanceJsonConverter.Settings);
}

public class Error
{
    [JsonProperty("code", NullValueHandling=NullValueHandling.Ignore)]
    public string Code { get; set; }

    [JsonProperty("message", NullValueHandling=NullValueHandling.Ignore)]
    public string Message { get; set; }
}