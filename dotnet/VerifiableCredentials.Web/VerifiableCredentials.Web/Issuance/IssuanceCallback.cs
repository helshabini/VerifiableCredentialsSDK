using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VerifiableCredentials.Web.Issuance;

public class IssuanceCallback
{
    [JsonProperty("requestId")]
    [JsonRequired]
    public Guid RequestId { get; set; }

    [JsonProperty("code")]
    [JsonRequired]
    [JsonConverter(typeof(StringEnumConverter))]
    public CallbackCode Code { get; set; }

    [JsonProperty("state")]
    [JsonRequired]
    public Guid State { get; set; }

    [JsonProperty("error", NullValueHandling=NullValueHandling.Ignore)]
    public IssuanceError? Error { get; set; }

    public static IssuanceCallback? FromJson(string json) =>
        JsonConvert.DeserializeObject<IssuanceCallback>(json, IssuanceJsonConverter.Settings);

    public string ToJson() => JsonConvert.SerializeObject(this, IssuanceJsonConverter.Settings);
}

public enum CallbackCode
{
    [EnumMember(Value = "request_retrieved")]
    RequestRetrieved,
    [EnumMember(Value = "issuance_successful")]
    IssuanceSuccessful,
    [EnumMember(Value = "Issuance_error")]
    IssuanceError
}