using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsToPictures.Models
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class SupportedFormatAttribyte : Attribute
    {
        readonly string format;
        public SupportedFormatAttribyte(string format)
        {
            this.format = format;
        }
        public string Format => format;
    }
}