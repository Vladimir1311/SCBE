using SituationCenterBackServer.Models.DocumentHandlingModels;
using SituationCenterBackServer.Models.StorageModels;
using System;
using System.Collections.Generic;
using IO = System.IO;

namespace SituationCenterBackServer.Services
{
    public interface IDocumentHandlerService
    {
        bool IsSupported(string format);
        (bool success, string message) SendDocumentToHandle(File document);
        void FillStates(IEnumerable<File> files);
        void FillState(File file);
        IO.Stream GetPicture(File filePath, int pageNum);
        event Action<File> NewPagesAvailable;
    }
}
