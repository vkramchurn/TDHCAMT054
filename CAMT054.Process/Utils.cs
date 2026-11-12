using System.Security.Cryptography;
using System.Text;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CAMT054.Process
{
    public class Utils
    {
        private static readonly byte[] key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes = AES-128
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("6543210987654321"); // 16 bytes
        public static T DeserializeXml<T>(string xmlContent)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringReader reader = new StringReader(xmlContent))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
        public static string DecodeFromBase64(string base64,string aesKey = "", string aesIv = "")
        {
            using Aes aes = Aes.Create();
            aes.Key = Convert.FromBase64String(aesKey);// Encoding.UTF8.GetBytes(aesKey);// key;
            aes.IV = Convert.FromBase64String(aesIv);// Encoding.UTF8.GetBytes(aesIv);// iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] inputBytes = Convert.FromBase64String(base64);

            byte[] decrypted = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
        public static string EncodeToBase64(string text, string aesKey = "", string aesIv = "")
        {
            using Aes aes = Aes.Create();
            aes.Key = Convert.FromBase64String(aesKey);// Encoding.UTF8.GetBytes(aesKey);// key;
            aes.IV = Convert.FromBase64String(aesIv);// Encoding.UTF8.GetBytes(aesIv);// iv;

            using var encryptor = aes.CreateEncryptor();
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);

            byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Convert.ToBase64String(encrypted);
        }
        public static string GetNamespace(string xml)
        {
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.MoveToContent();
            return reader.NamespaceURI;
        }
        public static object Deserialize(string xml,string ns)
        {
            return ns switch
            {
                "urn:iso:std:iso:20022:tech:xsd:camt.054.001.04"
                    => DeserializeXml<CAMT054.Process.Models.V04.Document>(xml),

                "urn:iso:std:iso:20022:tech:xsd:camt.054.001.08"
                    => DeserializeXml<CAMT054.Process.Models.V08.Document>(xml),

                _ => throw new NotSupportedException($"Unsupported namespace: {ns}")
            };
        }
        public static string GetDate(string dt)
        {
            return string.IsNullOrEmpty(dt) ? null : dt.Substring(0, 10);
        }
        public static string GetDate(DateTime? dt)
        {
            return dt==null?"" : dt.Value.ToString("yyyy-MM-dd");
        }
        public static int IfNotNullConvertToInt(string val)
        {
            return (val != null && val != "") ? Int32.Parse(val) : 0;
        }
    }
}
