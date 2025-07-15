using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Api.CommonFuncations
{
    public class md5
    {
        public static string Encode_md5(string password)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] res = md5.ComputeHash(Encoding.Default.GetBytes(password));
                byte[] result = md5.Hash;

                for (int i = 0; i < result.Length; i++)
                {
                    //change it into 2 hexadecimal digits
                    //for each byte
                    builder.Append(result[i].ToString("x2"));
                }
            }
            catch(Exception ex)
            {

            }
            

            return builder.ToString();
        }

        public static string ComputeSha256Hash(string rawData)
        {
            StringBuilder builder = new StringBuilder();
            // Create a SHA256   
            try
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                    // Convert byte array to a string   

                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }

                }
            }
            catch (Exception ex)
            {

                
            }
            
            return builder.ToString();
        }
    }
}