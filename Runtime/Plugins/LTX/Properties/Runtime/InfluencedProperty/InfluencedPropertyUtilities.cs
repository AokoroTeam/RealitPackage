using System;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ChanneledProperties
{
    public static class InfluencedPropertyUtilities
    {
        static Dictionary<Type, IBaseCalculator> calculators;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void InitializeOnLoad()
        {
            Debug.Log($"[Channeled properties] Initializing operators...");
            calculators = new Dictionary<Type, IBaseCalculator>
            {
                { typeof(float), new FloatCalculator() },
                { typeof(int), new IntCalculator() },
                { typeof(double), new DoubleCalculator() },
                { typeof(Vector2), new Vector2Calculator() },
                { typeof(Vector3), new Vector3Calculator() }
            };
        }

        public static void AddCalculator<T>(IBaseCalculator<T> calculator)
        {
            if (calculators.ContainsKey(typeof(T)))
                return;

            calculators.Add(typeof(T), calculator);
        }


        internal static T DoOperation<T>(T first, T second, Operation operation)
        {
            if(TryFindCalculator(out IBaseCalculator<T> calculator))
            {
                return operation switch
                {
                    Operation.Multiply => calculator.Multiply(first, second),
                    Operation.Divide => calculator.Divide(first, second),
                    Operation.Add => calculator.Add(first, second),
                    Operation.Subtract => calculator.Substract(first, second),
                    _ => first,
                };
            }

            return first;
        }
        private static bool TryFindCalculator<T>(out IBaseCalculator<T> calculator)
        {
            if (calculators.TryGetValue(typeof(T), out IBaseCalculator baseCalculator))
            {
                if(baseCalculator is IBaseCalculator<T> c)
                {
                    calculator = c;
                    return true;
                }
            }

            calculator = null;
            return false;
        }
    }
}
