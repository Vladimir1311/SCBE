using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Services
{
    public interface IConfigsManager
    {
        string CoreIP { get; set; }
    }
}
