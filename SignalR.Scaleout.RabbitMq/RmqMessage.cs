namespace SignalR.Scaleout.RabbitMq
{
    internal class RmqMessage
    {
        public int StreamIndex { get; set; }
        public byte[] Body { get; set; }
    }
}