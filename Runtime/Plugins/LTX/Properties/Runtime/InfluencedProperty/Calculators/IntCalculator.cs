namespace LTX.ChanneledProperties
{
    internal class IntCalculator : IBaseCalculator<int>
    {
        public int Add(int first, int second)
        {
            return first + second;
        }

        public int Divide(int first, int second)
        {
            return first / second;
        }

        public int Multiply(int first, int second)
        {
            return first * second;
        }

        public int Substract(int first, int second)
        {
            return first - second;
        }
    }
}
