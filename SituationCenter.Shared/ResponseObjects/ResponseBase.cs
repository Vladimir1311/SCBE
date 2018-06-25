using Newtonsoft.Json;
using SituationCenter.Shared.Exceptions;

namespace SituationCenter.Shared.ResponseObjects
{
    public class ResponseBase
    {
        [JsonProperty("status")]
        public StatusCode StatusCode { get; }

        [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
        public StatusCode[] Errors { get; }

        public ResponseBase()
        {
            StatusCode = StatusCode.OK;
        }

        protected ResponseBase(StatusCode[] statusCodes)
        {
            switch (statusCodes.Length)
            {
                case 0:
                    StatusCode = StatusCode.UnknownError;
                    break;

                case 1:
                    StatusCode = statusCodes[0];
                    break;

                default:
                    StatusCode = StatusCode.ComplexError;
                    Errors = statusCodes;
                    break;
            }
        }

        public static ResponseBase BadResponse(params StatusCode[] statusCodes) =>
            new ResponseBase(statusCodes);

        private static readonly ResponseBase goodResponse = new ResponseBase();
        public static ResponseBase OKResponse => goodResponse;
    }
}