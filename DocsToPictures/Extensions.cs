using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DocsToPictures
{
    public static class Extensions
    {
        private static readonly Regex _regexEncodedFileName = new Regex(@"^=\?utf-8\?B\?([a-zA-Z0-9/+]+={0,2})\?=$");

        public static bool IsExtended<T>(this Type type)
        {
            var compareType = typeof(T);
            while (type.BaseType != null)
            {
                if (type.BaseType == typeof(T))
                    return true;
                type = type.BaseType;
            }
            return false;
        }

        public static string TryToGetOriginalFileName(this string fileNameInput)
        {
            Match match = _regexEncodedFileName.Match(fileNameInput);
            if (match.Success && match.Groups.Count > 1)
            {
                string base64 = match.Groups[1].Value;
                try
                {
                    byte[] data = Convert.FromBase64String(base64);
                    return Encoding.UTF8.GetString(data);
                }
                catch (Exception)
                {
                    //ignored
                    return fileNameInput;
                }
            }
            return fileNameInput;
        }

    }
}