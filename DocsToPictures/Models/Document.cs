using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsToPictures.Models
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Folder { get; set; }
        public string Name { get; set; }
        public int Progress { get; set; }
        public string[] PagesPaths { get; set; }

    }
}