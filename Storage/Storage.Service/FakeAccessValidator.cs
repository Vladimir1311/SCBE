using SituationCenterBackServer.Interfaces;

namespace Storage.Service
{
    internal class FakeFalseAccessValidator : IAccessValidator
    {
        public bool CanAccessToFolder(string userToken, string targetFolder) =>
            false;
    }

    internal class FakeTrueAccessValidator : IAccessValidator
    {
        public bool CanAccessToFolder(string userToken, string targetFolder) =>
            true;
    }
}
