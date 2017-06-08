using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.StorageModels
{
    public interface IStorageManager
    {
        IEnumerable<Directory> GetRootDitionares();

    }
}
