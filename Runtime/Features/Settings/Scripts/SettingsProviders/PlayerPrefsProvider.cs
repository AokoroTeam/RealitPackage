using LTX.Settings;
using UnityEngine;

namespace Aokoro.Settings
{
    public class PlayerPrefsProvider : ISettingProvider
    {
        private const char VectorCoordinatesSeparator = ';';

        public PlayerPrefsProvider()
        {

        }

        public bool TryReadSetting(ref ISetting setting)
        {
            try
            {
                string internalName = setting.InternalName;

                if (!PlayerPrefs.HasKey(internalName))
                    return false;

                bool success = false;
                switch (setting.Type)
                {
                    case SettingType.Float:
                        if (setting is ISetting<float> f)
                        {
                            f.SetValue(PlayerPrefs.GetFloat(internalName));
                            success = true;
                        }
                        break;
                    case SettingType.Integer or SettingType.Choice:
                        if (setting is ISetting<int> i)
                        {
                            int value = PlayerPrefs.GetInt(internalName);
                            i.SetValue(value);
                            success = true;
                        }
                        break;
                    case SettingType.Text:
                        if (setting is ISetting<string> s)
                        {
                            s.SetValue(PlayerPrefs.GetString(internalName));
                            success = true;
                        }
                        break;
                    case SettingType.Boolean:
                        if (setting is ISetting<bool> b)
                        {
                            b.SetValue(PlayerPrefs.GetInt(internalName) == 1);
                            success = true;
                        }
                        break;
                    case SettingType.Vector3:
                        if (setting is ISetting<Vector3> v3)
                        {
                            string[] coordinates = PlayerPrefs.GetString(internalName).Split(';');
                            Vector3 vector = new(float.Parse(coordinates[0]),
                                                 float.Parse(coordinates[1]),
                                                 float.Parse(coordinates[2]));
                            v3.SetValue(vector);
                            success = true;
                        }
                        break;
                    case SettingType.Vector2:
                        if (setting is ISetting<Vector2> v2)
                        {
                            string[] coordinates = PlayerPrefs.GetString(internalName).Split(';');
                            Vector2 vector = new(float.Parse(coordinates[0]),
                                                 float.Parse(coordinates[1]));
                            v2.SetValue(vector);
                            success = true;
                        }
                        break;
                }

                return success;

            }
            catch
            {
                return false;
            }
        }

        public bool TryWriteSetting(ref ISetting setting)
        {
            bool dirty = true;

            switch (setting.Type)
            {
                case SettingType.Float:
                    if (setting is ISetting<float> f)
                        PlayerPrefs.SetFloat(f.InternalName, f.Value);
                    break;
                case SettingType.Integer or SettingType.Choice:
                    if (setting is ISetting<int> i)
                        PlayerPrefs.SetInt(i.InternalName, i.Value);
                    break;
                case SettingType.Text:
                    if (setting is ISetting<string> s)
                        PlayerPrefs.SetString(s.InternalName, s.Value);
                    break;
                case SettingType.Boolean:
                    if (setting is ISetting<bool> b)
                        PlayerPrefs.SetFloat(b.InternalName, b.Value ? 1 : 0);
                    break;
                case SettingType.Vector3:
                    // x;y;z
                    if (setting is ISetting<Vector3> v3)
                        PlayerPrefs.SetString(v3.InternalName,$"{v3.Value.x}{VectorCoordinatesSeparator}{v3.Value.y}{VectorCoordinatesSeparator}{v3.Value.z}");
                    break;
                case SettingType.Vector2:
                    if (setting is ISetting<Vector2> v2)
                        PlayerPrefs.SetString(v2.InternalName, $"{v2.Value.x}{VectorCoordinatesSeparator}{v2.Value.y}");
                    break;
                default:
                    dirty = false;
                    break;
            }

            if (dirty)
                PlayerPrefs.Save();

            return dirty;
        }
    }
}
