using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Litter.Support
{
    public class KeyGenerator
    {
        private const string keySymbols = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
        private static readonly Random rnd = new Random();
        public static string Generate()
        {
            var generatedUrl = "";

            for(int i = 0; i < 8; i++)
            {
                generatedUrl += keySymbols[rnd.Next(0, keySymbols.Length)];
            }

            return "https://localhost:5001/" + generatedUrl;
        }
    }
}
