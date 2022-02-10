using System;

namespace Jodit.Models
{
    public class KeyGenerator
    {
        
        public static string GetRandomString()
        {
            string bigString = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";

            Random rnd = new Random();
            string str = "";
 
            for (int i=0; i<25; i++)
            {
                str += (char) rnd.Next(0, bigString.Length-1);
            }
            return str;
        }
    }
}