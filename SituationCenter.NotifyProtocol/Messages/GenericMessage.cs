namespace SituationCenter.NotifyProtocol.Messages
{
	public class GenericMessage<T> : Message
    {
        public T Data { get; set; }
    }
}
