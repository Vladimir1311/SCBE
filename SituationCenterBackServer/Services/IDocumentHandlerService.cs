using SituationCenterBackServer.Models.DocumentHandlingModels;
using SituationCenterBackServer.Models.StorageModels;
using System.Collections.Generic;

namespace SituationCenterBackServer.Services
{
    public interface IDocumentHandlerService
    {
        FileReadyState StateOf(string pathToFile);
        bool IsSupported(string format);
        (bool success, string message) SendDocumentToHandle(File document);
        void FillStates(IEnumerable<File> files);
        void FillState(File file);
    }
}
