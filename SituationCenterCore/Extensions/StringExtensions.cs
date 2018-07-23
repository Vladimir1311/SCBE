using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Extensions
{
    public static class StringExtensions
    {
        private const string Numbers = "0123456789";
        private const string Symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string allSymbols;
        private static string Chars => allSymbols ?? (allSymbols = Symbols + Symbols.ToLower() + Numbers);
        public static string Random(this string str, int length)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));
            var rand = new Random();
            return new string(Enumerable.Repeat(Chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}
