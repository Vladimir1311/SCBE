using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocsToPictures.Models
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class SupportedFormatAttribyte : Attribute
    {
        readonly string format;
        public SupportedFormatAttribyte(string format)
        {
            this.format = format;
        }
        public string Format => format;
    }
}
