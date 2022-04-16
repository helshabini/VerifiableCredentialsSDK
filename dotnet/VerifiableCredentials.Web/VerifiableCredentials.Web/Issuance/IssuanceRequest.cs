using System.Globalization;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;

namespace VerifiableCredentials.Web.Issuance;

public class IssuanceRequest
    {
        public IssuanceRequest(IssuanceRequestOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Authority))
            {
                throw new ArgumentException("DIDAuthority is required.", nameof(options.Authority));
            }

            if (string.IsNullOrWhiteSpace(options.ClientName))
            {
                throw new ArgumentException("ClientName is required.", nameof(options.ClientName));
            }
            
            if (string.IsNullOrWhiteSpace(options.CredentialType))
            {
                throw new ArgumentException("CredentialType is required.", nameof(options.CredentialType));
            }
            
            if (string.IsNullOrWhiteSpace(options.TenantId))
            {
                throw new ArgumentException("TenantId is required.", nameof(options.TenantId));
            }
            
            IncludeQrCode = options.IncludeQrCode;
            Authority = options.Authority;

            Registration = new RequestRegistration
            {
                ClientName = options.ClientName,
                LogoUrl = string.IsNullOrEmpty(options.LogoUrl) ?  null : new Uri(options.LogoUrl),
                TermsOfServiceUrl = string.IsNullOrEmpty(options.TermsOfServiceUrl) ?  null : new Uri(options.TermsOfServiceUrl),
            };

            Issuance = new Issuance
            {
                Type = options.CredentialType,
                Manifest = string.Format(CultureInfo.InvariantCulture, Constants.Manifest, options.TenantId,
                    HttpUtility.UrlEncode(options.CredentialType))
            };

            Options = options;
        }

        [JsonIgnore]
        public IssuanceRequestOptions Options { get; set; }
        
        [JsonProperty("includeQRCode")]
        [JsonRequired]
        public bool IncludeQrCode { get; set; }

        [JsonProperty("callback")]
        [JsonRequired]
        public Callback Callback { get; set; }

        [JsonProperty("authority")]
        [JsonRequired]
        public string Authority { get; set; }

        [JsonProperty("registration")]
        [JsonRequired]
        public RequestRegistration Registration { get; set; }

        [JsonProperty("issuance")]
        [JsonRequired]
        public Issuance Issuance { get; set; }
        
        public object Clone() {
            return MemberwiseClone();
        }
        public static IssuanceRequest? FromJson(string json) => JsonConvert.DeserializeObject<IssuanceRequest>(json, IssuanceJsonConverter.Settings);
    }

    public class Callback
    {
        [JsonProperty("url")]
        [JsonRequired]
        public Uri Url { get; set; }

        [JsonProperty("state")]
        [JsonRequired]
        public string State { get; set; }

        [JsonProperty("headers", NullValueHandling=NullValueHandling.Ignore)]
        public Headers Headers { get; set; }
    }

    public class Headers
    {
        [JsonProperty("api-key", NullValueHandling=NullValueHandling.Ignore)]
        public string ApiKey { get; set; }
        
        [JsonProperty("authorization", NullValueHandling=NullValueHandling.Ignore)]
        public string Authorization { get; set; }
    }

    public class Issuance
    {
        [JsonProperty("type")]
        [JsonRequired]
        public string Type { get; set; }

        [JsonProperty("manifest")]
        [JsonRequired]
        public string Manifest { get; set; }

        [JsonProperty("pin", NullValueHandling=NullValueHandling.Ignore)]
        public Pin Pin { get; set; }

        [JsonProperty("claims", NullValueHandling=NullValueHandling.Ignore)]
        public Dictionary<string, string> Claims { get; set; }

        // public Issuance(string type, string manifest, bool isMobile, Dictionary<string,string> claims)
        // {
        //     Type = type;
        //     Manifest = manifest;
        //     if (isMobile || claims is {Count: 0})
        //     {
        //         Pin = null;
        //     }
        //     else
        //     {
        //         Pin = Pin.Generate(4);
        //     }
        // }
    }

    public enum PinType
    {
        [JsonProperty("numeric")]
        Numeric
    }

    public enum PinAlgorithm
    {
        [JsonProperty("sha256")]
        Sha256
    }

    public class Pin
    {
        [JsonProperty("value")]
        [JsonRequired]
        public string Value { get; set; }
        
        [JsonProperty("type", NullValueHandling=NullValueHandling.Ignore)]
        public PinType Type { get; set; }

        [JsonProperty("length")]
        [JsonRequired]
        public int Length { get; set; }

        [JsonProperty("salt", NullValueHandling=NullValueHandling.Ignore)]
        public string Salt { get; set; }
        
        [JsonProperty("alg", NullValueHandling=NullValueHandling.Ignore)]
        public PinAlgorithm Algorithm { get; set; }
        
        [JsonProperty("iterations", NullValueHandling=NullValueHandling.Ignore)]
        public int Iterations { get; set; }
        
        public static Pin Generate(int length)
        {
            var pinMaxValue = (int)Math.Pow(10, length) - 1;
            var randomNumber = RandomNumberGenerator.GetInt32(1, pinMaxValue);
            
            return new Pin()
            {
                Value = string.Format("{0:D" + length + "}", randomNumber),
                Length = length
            };
        }
    }

    public class RequestRegistration
    {
        [JsonProperty("clientName")]
        [JsonRequired]
        public string ClientName { get; set; }
        
        [JsonProperty("logoUrl", NullValueHandling=NullValueHandling.Ignore)]
        public Uri? LogoUrl { get; set; }
        
        [JsonProperty("termsOfServiceUrl", NullValueHandling=NullValueHandling.Ignore)]
        public Uri? TermsOfServiceUrl { get; set; }
    }