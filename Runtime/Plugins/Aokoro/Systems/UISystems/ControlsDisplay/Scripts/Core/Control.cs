using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.UI.ControlsDiplaySystem
{
    [System.Serializable]
    public struct Control : IEnumerable<Control.ControlData>
    {
        public readonly struct ControlData
        {
            public readonly string Device;
            public readonly string Path;
            public readonly string DisplayName;

            internal ControlData(string path, string displayName, string device)
            {
                Path = path;
                DisplayName = displayName;
                Device = device;
            }

            public static implicit operator Control(Control.ControlData data) => new Control(data.Path, data.DisplayName, data.Device);
        }
        public bool IsComposite => Lenght > 1;
        public string Path => controls[0].Path;
        public string Device => controls[0].Device;
        public string DisplayName => controls[0].DisplayName;
        public int Lenght => controls.Count;

        public string compositeType;

        private List<ControlData> controls;


        public Control(string path, string displayName, string device)
        {
            this.compositeType = string.Empty;

            controls = new List<ControlData>() { new ControlData(path, displayName, device) };
        }

        public Control(string compositeType)
        {
            this.compositeType = compositeType;
            controls = new List<ControlData>();
        }

        public void AddControl(string path, string displayName, string device)
        {
            controls.Add(new ControlData(path, displayName, device));
        }

        public string GetPathAtIndex(int index) => controls[index].Path;
        public string GetDisplayNameAtIndex(int index) => controls[index].DisplayName;
        public string GetDeviceAtIndex(int index) => controls[index].Device;
        public ControlData GetDataAtIndex(int index) => controls[index];

        public Control[] Split()
        {
            if (!IsComposite)
                return new Control[] { this };
            else
            {
                Control[] controlArray = new Control[Lenght];
                for (int i = 0; i < Lenght; i++)
                    controlArray[i] = GetDataAtIndex(i);

                return controlArray;
            }
        }

        public IEnumerator<ControlData> GetEnumerator() => controls.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => controls.GetEnumerator();
    }
}
