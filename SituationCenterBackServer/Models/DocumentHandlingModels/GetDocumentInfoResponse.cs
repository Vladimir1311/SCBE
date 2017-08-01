using Common.ResponseObjects;

namespace SituationCenterBackServer.Models.DocumentHandlingModels
{
    public class GetDocumentInfoResponse : ResponseBase
    {
#pragma warning disable CS0109 // The member 'GetDocumentInfoResponse.Object' does not hide an accessible member. The new keyword is not required.
        public new DocumentInfo Object { get; set; }
#pragma warning restore CS0109 // The member 'GetDocumentInfoResponse.Object' does not hide an accessible member. The new keyword is not required.
    }
}