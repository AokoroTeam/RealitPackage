namespace LTX.ChanneledProperties
{
    internal class DoubleCalculator : IBaseCalculator<double>
    {
        public double Add(double first, double second)
        {
            return first + second;
        }

        public double Divide(double first, double second)
        {
            return first / second;
        }

        public double Multiply(double first, double second)
        {
            return first * second;
        }

        public double Substract(double first, double second)
        {
            return first - second;
        }
    }
}
