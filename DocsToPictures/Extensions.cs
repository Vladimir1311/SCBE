using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsToPictures
{
    public static class Extensions
    {
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
    }
}