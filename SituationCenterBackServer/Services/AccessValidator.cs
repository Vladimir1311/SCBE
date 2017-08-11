using SituationCenterBackServer.Interfaces;

namespace SituationCenterBackServer.Services
{
    public class AccessValidator : IAccessValidator
    {
        public bool CanAccessToFolder(string userToken, string targetFolder) => true;
    }
}
