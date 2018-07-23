using System;
namespace SituationCenterCore.Models.RealTime
{
	public class GenericMessage<T> : Message
    {
        public T Data { get; set; }
    }
}
