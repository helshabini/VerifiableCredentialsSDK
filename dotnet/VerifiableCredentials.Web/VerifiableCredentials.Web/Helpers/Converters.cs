using System.Runtime.Serialization.Formatters.Binary;

namespace VerifiableCredentials.Web.Helpers;

public static class Converters
{
    // Convert an object to a byte array
    public static byte[]? JsonToByteArray(string json)
    {
        if(string.IsNullOrEmpty(json))
            return null;

        MemoryStream ms = new MemoryStream();
        BinaryWriter br = new BinaryWriter(ms);
        br.Write(json);

        return ms.ToArray();
    }

// Convert a byte array to an Object
    public static Object ByteArrayToObject(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        Object obj = (Object) binForm.Deserialize(memStream);

        return obj;
    }
}