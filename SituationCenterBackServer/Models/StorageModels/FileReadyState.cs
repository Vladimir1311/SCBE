﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.StorageModels
{
    public enum FileReadyState
    {
        Unknow,
        InQueue,
        Handling,
        Ready
    }
}
