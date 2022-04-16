using Newtonsoft.Json;

namespace VerifiableCredentials.Web.Issuance;

public class IssuanceCallback
{
    [JsonProperty("requestId")]
    [JsonRequired]
    public Guid RequestId { get; set; }

    [JsonProperty("code")]
    [JsonRequired]
    public CallbackCode Code { get; set; }

    [JsonProperty("state")]
    [JsonRequired]
    public Guid State { get; set; }

    [JsonProperty("error", NullValueHandling=NullValueHandling.Ignore)]
    public IssuanceError Error { get; set; }

    public static IssuanceCallback FromJson(string json) =>
        JsonConvert.DeserializeObject<IssuanceCallback>(json, IssuanceJsonConverter.Settings);
}

public enum CallbackCode
{
    [JsonProperty("request_retrieved")]
    RequestRetrieved,
    [JsonProperty("issuance_successful")]
    IssuanceSuccessful,
    [JsonProperty("Issuance_error")]
    IssuanceError
}