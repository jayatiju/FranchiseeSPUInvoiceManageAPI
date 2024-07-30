using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace WebApplication1.Models
{
    public class CertificateEncryption
    {
        public static byte[] GetNewSessionKey()
        {
            using (Aes myAes = Aes.Create("AES"))
            {
                myAes.KeySize = 256;
                myAes.GenerateKey();
                return myAes.Key;
            }
        }

        public static string EncryptWithPublicKey(byte[] stringToEncrypt, string cert)
        {
            X509Certificate2 certificate;
            certificate = new X509Certificate2(cert);
            RSA rsa = certificate.GetRSAPublicKey();
            byte[] cipher = rsa.Encrypt(stringToEncrypt, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(cipher);
        }

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

        public static byte[] ConvertStringToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public static byte[] Generatehash256(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashString = new SHA256Managed();
            var hashValue = hashString.ComputeHash(message);
            return hashValue;
        }
    }
}