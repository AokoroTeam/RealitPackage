namespace LTX.ChanneledProperties
{
    public interface IChannel<T>
    {
        public ChannelKey Key { get; set; }
        public T Value { get; set; }
    }
}
