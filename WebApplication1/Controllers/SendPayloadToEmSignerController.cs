using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Windows;
using WebApplication1.Models;
using WebApplication1.InvoiceSAPReference;
using System.ServiceModel;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Helpers;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class SendPayloadToEmSignerController : ApiController
    {
        
        
        [HttpPost]
        public EmSignerApiOutput SendPayloadToEmSigner([FromBody] EmSignerApiInput emSignerApiInput)
        {
            EmSignerApiOutput emSignerApiOutput = new EmSignerApiOutput();

            //encrypted session key output
            byte[] sessionKey = GetNewSessionKey();
            string EncryptedSessionKey = Encrypt(sessionKey);
            emSignerApiOutput.encryptedSessionKey = EncryptedSessionKey;

            //getting the pdf file into a base64 integer
            String fileLocation = emSignerApiInput.FileLocations[0];

            if (string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
            {
                //create error response
            }

            byte[] pdfBytes = File.ReadAllBytes(fileLocation);
            string base64Pdf = Convert.ToBase64String(pdfBytes);

            //creating json payload
            JsonDataEmSigner jsonDataEmSigner = new JsonDataEmSigner();

            jsonDataEmSigner.FileType = "PDF";
            jsonDataEmSigner.File = base64Pdf;
            jsonDataEmSigner.ReferenceNumber = emSignerApiInput.OrderId[0];
            jsonDataEmSigner.Name = emSignerApiInput.vendorname;
            jsonDataEmSigner.AuthToken = "5e1586b6-1196-476a-af63-06b380a9dc15";
            jsonDataEmSigner.SignatureType = 3;
            jsonDataEmSigner.SelectPage = "ALL";
            jsonDataEmSigner.SignaturePosition = "Bottom-Right";
            jsonDataEmSigner.SUrl = "http://localhost:44361/test.aspx";
            jsonDataEmSigner.FUrl = "http://localhost:44361/test.aspx";
            jsonDataEmSigner.CUrl = "http://localhost:44361/test.aspx";
            jsonDataEmSigner.IsGSTN = false;
            jsonDataEmSigner.IsGSTN3B = false;

            byte[] jsonPayLoadToBeEncrypted = SerializeToByteArray(jsonDataEmSigner);
            byte[] encryptedJsonPayload = EncryptDataAES(jsonPayLoadToBeEncrypted, sessionKey);

            string jsonPayload = Convert.ToBase64String(encryptedJsonPayload);
            emSignerApiOutput.encryptedJsonData = jsonPayload;



            //encrypted hash output
            string  jsonForHash = JsonConvert.SerializeObject(jsonDataEmSigner);
            //string jsonForHash = Convert.ToBase64String(jsonPayLoadToBeEncrypted);
            //string jsonForHash = jsonDataEmSigner.ToString();
            byte[] hash = Generatehash256(jsonForHash);
            byte[] encryptedHash = EncryptDataAES(hash, sessionKey);
            emSignerApiOutput.encryptedHash = Convert.ToBase64String(encryptedHash);

            return emSignerApiOutput;
        }

        //object getting serialized to byte array
        public static byte[] SerializeToByteArray(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
        //new session key
        public static byte[] GetNewSessionKey()
        {
            using (Aes myAes = Aes.Create("AES"))
            {
                myAes.KeySize = 256;
                myAes.GenerateKey();
                return myAes.Key;
            }
        }

        //encrypt session key
        public static string EncryptWithPublicKey(byte[] stringToEncrypt)
        {
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate;
            certificate = new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "\\Certificate_C3.cer"); //AppDomain.CurrentDomain.BaseDirectory +
            byte[] cipherbytes = Convert.FromBase64String(Convert.ToBase64String(stringToEncrypt));
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            byte[] cipher = rsa.Encrypt(cipherbytes, false);
            return Convert.ToBase64String(cipher);
        }

        public static string Encrypt(byte[] PlainBytes)

        {
            System.Security.Cryptography.X509Certificates.X509Certificate2 Certificate;
            Certificate = new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "\\Certificate_C3.cer");
            using (RSA rSA = Certificate.GetRSAPublicKey())

            {

                return Convert.ToBase64String(rSA.Encrypt(PlainBytes, RSAEncryptionPadding.Pkcs1));

            }

        }
        //encrypt json payload and or the hash
        public static byte[] EncryptDataAES(byte[] toEncrypt, byte[] key)
        {
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/ECB/PKCS7");
            cipher.Init(true, new KeyParameter(key));
            int outputSize = cipher.GetOutputSize(toEncrypt.Length);
            byte[] tempOP = new byte[outputSize];
            int processLen = cipher.ProcessBytes(toEncrypt, 0, toEncrypt.Length, tempOP, 0);
            int outputLen = cipher.DoFinal(tempOP, processLen);
            byte[] result = new byte[processLen + outputLen];
            System.Array.Copy(tempOP, 0, result, 0, result.Length);
            return result;
        }

        //create hash
        public static byte[] Generatehash256(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashString = new SHA256Managed();
            string hex = "";
            var hashValue = hashString.ComputeHash(message);
            return hashValue;
        }

        
    }
}