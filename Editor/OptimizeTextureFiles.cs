using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace Realit.Editor
{
    public static class OptimizeTextureFiles
    {
        [MenuItem("Assets/Aokoro/Optimize texture in folder")]
        public static void Optimze()
        {
            if (Selection.activeObject != null)
            {
                var folderPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

                if (!string.IsNullOrWhiteSpace(folderPath))
                {
                    OptimizeTextureInFolder(folderPath);
                }
            }
        }

        public static void OptimizeTextureInFolder(params string[] folders)
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture", folders);

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];
                string oldPath = AssetDatabase.GUIDToAssetPath(guid);

                string ext = Path.GetExtension(oldPath);
                if (ext == ".tga")
                {
                    stringBuilder.Clear();
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(oldPath);

                    var data = texture.EncodeToPNG();
                    stringBuilder.Append(oldPath);
                    stringBuilder.Replace(ext, ".png", oldPath.Length - ext.Length, ext.Length);

                    string newPath = stringBuilder.ToString();
                    Debug.Log($"Replacing {oldPath} with {newPath}");
                    
                    //File.Delete(oldPath);
                    //File.WriteAllBytes(newPath, data);
                    //File.Move($"{oldPath}.meta", $"{newPath}.meta");
                }
            }
        }
    }
}