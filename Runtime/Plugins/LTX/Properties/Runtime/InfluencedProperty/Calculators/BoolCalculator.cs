
namespace LTX.ChanneledProperties
{
    public class BoolCalculator : IBaseCalculator<bool>
    {
        public bool Add(bool first, bool second)
        {
            return first || second;
        }

        public bool Substract(bool first, bool second)
        {
            return first ^ second;
        }

        public bool Divide(bool first, bool second)
        {
            return first && second;
        }

        public bool Multiply(bool first, bool second)
        {
            return first && second;
        }
    }
}
