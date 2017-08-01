using Common.ResponseObjects;

namespace SituationCenterBackServer.Models.DocumentHandlingModels
{
    public class DocimentAddedResponse : ResponseBase
    {
        public new DocumentAddedObject Object { get; set; }
    }
}