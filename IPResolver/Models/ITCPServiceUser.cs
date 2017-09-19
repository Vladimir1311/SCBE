using System;

namespace IPResolver.Models
{
    public interface IPingable
    {
        DateTime ConnectionTime { get; set; }
        DateTime LastPing { get; set; }
    }
}