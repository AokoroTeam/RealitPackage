using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Realit.Editor
{
    public static class RealitContextMenu 
    {
        [MenuItem("Assets/Realit/Optimize mesh")]
        public static void OptimizeMeshes()
        {
            var guids = Selection.assetGUIDs;
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                Type t = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                if (t == typeof(Mesh))
                    InternalOptimizeMesh(AssetDatabase.LoadMainAssetAtPath(assetPath) as Mesh);
            }

            AssetDatabase.SaveAssets();
        }

        private static void InternalOptimizeMesh(Mesh mesh)
        {
            MeshUtility.Optimize(mesh);
            MeshUtility.SetMeshCompression(mesh, ModelImporterMeshCompression.High);
        }
    }

}