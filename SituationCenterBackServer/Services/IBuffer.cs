﻿using SituationCenterBackServer.Models.StorageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Services
{
    public interface IBuffer
    {
        string GetLinkFor(File file);
        string ServLink { get; }
    }
}
