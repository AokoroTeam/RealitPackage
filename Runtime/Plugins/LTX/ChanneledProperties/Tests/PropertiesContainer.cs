using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace LTX.ChanneledProperties.Tests
{
    public class PropertiesContainer : MonoBehaviour
    {
        private ChanneledProperty<float>[] properties = new ChanneledProperty<float>[0];

        [SerializeField, Range(1, 10000)]
        int propertiesCount = 100;

        public int PropertiesCount { get => propertiesCount; }

        private void OnValidate()
        {
            properties = new ChanneledProperty<float>[PropertiesCount];
            for (int i = 0; i < PropertiesCount; i++)
                properties[i] = new ChanneledProperty<float>(-1);
        }

        [Button]
        public void AddChannelToAllProperties()
        {
            Profiler.BeginSample("Add channels");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < PropertiesCount; i++)
                properties[i].AddChannel(this, PriorityTags.Smallest);

            stopwatch.Stop();
            Debug.Log(stopwatch.ElapsedMilliseconds);
            Profiler.EndSample();
            Debug.Break();
        }

        [Button]
        public void RemoveChannelToAllProperties()
        {
            Profiler.BeginSample("Remove channels");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < PropertiesCount; i++)
                properties[i].RemoveChannel(this);
            stopwatch.Stop();

            Debug.Log(stopwatch.ElapsedMilliseconds);
            Profiler.EndSample();
            Debug.Break();
        }

        [SerializeField]
        private float Value;

        [Button]
        public void SetValueToAllProperties()
        {
            Profiler.BeginSample("Write into channels");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            for (int i = 0; i < PropertiesCount; i++)
            {
                if (properties[i].HasChannel(this))
                    properties[i].Write(this, Value);
            }
            stopwatch.Stop();
            Debug.Log(stopwatch.ElapsedMilliseconds);
            Profiler.EndSample();
            Debug.Break();
        }

        [Button]
        public void EvaluateValueForAllProperties()
        {
            Profiler.BeginSample("Force evaluate channels");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            float v = 0;
            for (int i = 0; i < PropertiesCount; i++)
                v = properties[i].Value;
            
            stopwatch.Stop();
            Debug.Log(stopwatch.ElapsedMilliseconds);
            Profiler.EndSample();
            Debug.Break();
        }
    }
}
