using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VerifiableCredentials.Web.Issuance;

internal static class IssuanceJsonConverter
{
    public static readonly JsonSerializerSettings? Settings = new()
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
        },
    };
}