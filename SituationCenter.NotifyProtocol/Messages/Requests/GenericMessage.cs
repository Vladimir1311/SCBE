namespace SituationCenter.NotifyProtocol.Messages.Requests
{
	public class GenericMessage<T> : Message
    {
        public T Data { get; set; }
    }
}
