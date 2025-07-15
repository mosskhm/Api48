using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Api.CommonFuncations
{
    public class Base64
    {

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                    {
                        sb.Append(c);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            
            return sb.ToString();
        }

        public static string RemoveAccents(string str)
        {
            string result = str;
            try
            {
                byte[] tempBytes;
                tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(str);
                result = System.Text.Encoding.UTF8.GetString(tempBytes);
            }
            catch (Exception ex)
            {

            }
            
            return result;
        }
        public static string EncodeDecodeBase64(string mystring, int enc_dec)
        {
            string result = "";
            try
            {
                if (enc_dec == 1)
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(mystring);
                    result = System.Convert.ToBase64String(plainTextBytes);
                }
                else
                {
                    var base64EncodedBytes = System.Convert.FromBase64String(mystring);
                    result = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                }
            }
            catch(Exception ex)
            {

            }
            
            return result;
        }

        public static string Reverse(string s)
        {
            
            try
            {
                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            } 
            catch (Exception ex) 
            {
                return s;
            }

            
        }

        public static string CreateRandomStarting(int length)
        {
            try
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[length];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                var finalString = new String(stringChars);

                return finalString;
            }  
            catch(Exception ex)
            {
                return "FA123456";
            }
            
        }
    }
}