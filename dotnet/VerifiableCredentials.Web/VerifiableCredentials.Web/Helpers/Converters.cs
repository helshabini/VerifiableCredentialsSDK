using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace VerifiableCredentials.Web.Helpers;

public static class Converters
{
    // Convert an object to a byte array
    public static byte[]? JsonToByteArray(string json)
    {
        if(string.IsNullOrEmpty(json))
            return null;

        return JsonSerializer.SerializeToUtf8Bytes(json);
    }

// Convert a byte array to an Object
    public static string? ByteArrayToJson(byte[] bytes)
    {
        if (bytes.Length == 0)
            return null;

        return JsonSerializer.Deserialize<string>(bytes);
    }
}