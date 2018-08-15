using System;
using System.Collections.Generic;
using System.Text;

namespace SituationCenter.NotifyProtocol.Messages.Responses
{
    public class GenericTopicResponse<T> : TopicResponse
    {
        public T Data { get; set; }
    }
}
