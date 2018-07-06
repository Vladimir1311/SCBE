using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Extensions
{
    public static class StringExtensions
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static string Random(this string str, int length)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));
            var rand = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}
