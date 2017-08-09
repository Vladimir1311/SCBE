using SituationCenterBackServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Services
{
    public class AccessValidator : IAccessValidator
    {
        public bool CanAccessToFolder(string userToken, string targetFolder)
        {
            return userToken == targetFolder;
        }
    }
}
