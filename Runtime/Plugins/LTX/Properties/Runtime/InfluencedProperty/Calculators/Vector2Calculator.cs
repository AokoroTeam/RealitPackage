using UnityEngine;

namespace LTX.ChanneledProperties
{
    internal class Vector2Calculator : IBaseCalculator<Vector2>
    {
        public Vector2 Add(Vector2 first, Vector2 second)
        {
            return first + second;
        }
        public Vector2 Substract(Vector2 first, Vector2 second)
        {
            return first - second;
        }

        public Vector2 Divide(Vector2 first, Vector2 second)
        {
            return new(first.x / second.x, first.y / second.y);
        }

        public Vector2 Multiply(Vector2 first, Vector2 second)
        {
            return new(first.x * second.x, first.y * second.y);
        }

    }
}
