using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class SHA1Enc
    {
        public static string Sha1_Encrypt(string text, ref List<LogLines> lines)
        {
            string hash = "";
            try
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] combined = encoder.GetBytes(text);
                hash = BitConverter.ToString(sha.ComputeHash(combined)).Replace("-", "");
            }
            catch (Exception e)
            {
                lines = Add2Log(lines, e.ToString(), 100, lines[0].ControlerName);
            }
           
            return hash;
        }
        

    }
}