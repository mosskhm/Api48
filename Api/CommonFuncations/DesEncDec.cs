using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Api.CommonFuncations
{
    public class DesEncDec
    {
        

        public static string encrypt(string clearText, string secretKey)
        {
            try
            {
                TripleDESCryptoServiceProvider tripleDESProvider = new TripleDESCryptoServiceProvider();
                byte[] byteKey = Encoding.UTF8.GetBytes(secretKey.PadRight(24, '\0'));
                if (byteKey.Length > 24)
                {
                    byte[] bytePass = new byte[24];
                    Buffer.BlockCopy(byteKey, 0, bytePass, 0, 24);
                    byteKey = bytePass;
                }
                byte[] byteText = Encoding.UTF8.GetBytes(clearText);

                tripleDESProvider.Key = byteKey;
                tripleDESProvider.Mode = CipherMode.ECB;

                byte[] byteMessage = tripleDESProvider.CreateEncryptor().TransformFinalBlock(byteText, 0, byteText.Length);

                return Convert.ToBase64String(byteMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string decrypt(string data, string secretKey)
        {
            try
            {
                byte[] byteData = Convert.FromBase64String(data);
                byte[] byteKey = Encoding.UTF8.GetBytes(secretKey.PadRight(24, '\0'));
                if (byteKey.Length > 24)
                {
                    byte[] bytePass = new byte[24];
                    Buffer.BlockCopy(byteKey, 0, bytePass, 0, 24);
                    byteKey = bytePass;
                }

                TripleDESCryptoServiceProvider tripleDESProvider = new TripleDESCryptoServiceProvider();
                tripleDESProvider.Key = byteKey;
                tripleDESProvider.Mode = CipherMode.ECB;

                byte[] byteText = tripleDESProvider.CreateDecryptor().TransformFinalBlock(byteData, 0, byteData.Length);
                return Encoding.UTF8.GetString(byteText);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}