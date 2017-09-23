using System;

namespace CCF.Shared
{
    public enum MessageType : byte
    {
        Message,
        Result,
        PingRequest,
        PingResponse,
        ServiceCreateRequest,
        ServiceCreateResponse
    }
}
