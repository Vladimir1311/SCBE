using SituationCenterBackServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Services
{
    public class AlwaysTrueAccessValidator : IAccessValidator
    {
        public bool CanAccessToFolder(string userToken, string targetFolder) =>
            true;
    }
}
