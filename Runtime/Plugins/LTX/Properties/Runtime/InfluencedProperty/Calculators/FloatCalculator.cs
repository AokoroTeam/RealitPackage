namespace LTX.ChanneledProperties
{
    internal class FloatCalculator : IBaseCalculator<float>
    {
        public float Add(float first, float second)
        {
            return first + second;
        }

        public float Substract(float first, float second)
        {
            return first - second;
        }

        public float Divide(float first, float second)
        {
            return first / second;
        }

        public float Multiply(float first, float second)
        {
            return first * second;
        }

    }
}
