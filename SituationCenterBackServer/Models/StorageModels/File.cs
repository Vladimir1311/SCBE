using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.StorageModels
{
    public class File : IStorageEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public FileReadyState State { get; set; }
        public int Progress { get; set; }
    }
}
