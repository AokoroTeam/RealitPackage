namespace LTX.ChanneledProperties
{
    public interface IBaseCalculator<T> : IBaseCalculator
    {
        public T Add(T first, T second);
        public T Substract(T first, T second);
        public T Divide(T first, T second);
        public T Multiply(T first, T second);
    }

    public interface IBaseCalculator
    {

    }
}
