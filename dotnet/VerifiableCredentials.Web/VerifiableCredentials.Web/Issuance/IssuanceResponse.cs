using Newtonsoft.Json;

namespace VerifiableCredentials.Web;

public class IssuanceResponse
{
    [JsonProperty("requestId")]
    [JsonRequired]
    public Guid RequestId { get; set; }

    [JsonProperty("url")] 
    public string Url { get; set; }

    [JsonProperty("expiry")] 
    public int Expiry { get; set; }

    [JsonProperty("qrCode")] 
    public string QrCode { get; set; }

    [JsonProperty("date")]
    public string Date { get; set; }

    [JsonProperty("error")]
    public Error Error { get; set; }
    
    public static IssuanceResponse FromJson(string json) =>
        JsonConvert.DeserializeObject<IssuanceResponse>(json, IssuanceJsonConverter.Settings);
}

public class Error
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}