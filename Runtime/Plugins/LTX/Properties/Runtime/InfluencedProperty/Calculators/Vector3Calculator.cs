using UnityEngine;

namespace LTX.ChanneledProperties
{
    internal class Vector3Calculator : IBaseCalculator<Vector3>
    {
        public Vector3 Add(Vector3 first, Vector3 second)
        {
            return first + second;
        }

        public Vector3 Divide(Vector3 first, Vector3 second)
        {

            return new(first.x / second.x, first.y / second.y, first.z / second.z);
        }

        public Vector3 Multiply(Vector3 first, Vector3 second)
        {
            return new(first.x * second.x, first.y * second.y, first.z * second.z);
        }

        public Vector3 Substract(Vector3 first, Vector3 second)
        {
            return first - second;
        }
    }
}
