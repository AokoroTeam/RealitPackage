using Realit.Core;
using Realit.Core.Scenes;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace Realit.Editor
{
    public class RealitWindow : EditorWindow
    {
        private RealitBuildProfile buildProfile;

        private bool UsePreset;
        private string buildName;

        private bool hasMainMenu;
        private Dictionary<RealitScene, bool> scenes = new();

        [MenuItem("Tools/Aokoro/Realit")]
        public static void ShowWindw()
        {
            GetWindow<RealitWindow>("Realit");
        }

        void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            {
                //EditorGUILayout.PrefixLabel("Preset");
                var newProfile = (RealitBuildProfile)EditorGUILayout.ObjectField(buildProfile, typeof(RealitBuildProfile), false);
                if (newProfile != buildProfile)
                {
                    if (newProfile != null)
                    {
                        buildProfile = newProfile;
                        SetValues();
                    }
                    else
                        buildProfile = null;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (buildProfile != null)
            {

                SetValues();
                EditorGUILayout.LabelField("Scenes");
                int lastIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                foreach (var scene in scenes)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Height(25), GUILayout.ExpandWidth(false));
                    string sceneName = scene.Key.SceneName;

                    EditorGUILayout.LabelField(sceneName, GUILayout.ExpandWidth(false));

                    if (!scene.Value)
                        EditorGUILayout.LabelField("Scene not valid", GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false));

                    EditorGUILayout.EndHorizontal();
                }
                hasMainMenu = GUILayout.Toggle(hasMainMenu, "Main Menu");

                EditorGUI.indentLevel--;
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Remove"))
                {
                    scenes.Remove(scenes.Last().Key);
                    buildProfile.scenes = scenes.Keys.ToArray();
                }
                if (GUILayout.Button("Add"))
                {
                    scenes.Add(new RealitScene(), false);
                    buildProfile.scenes = scenes.Keys.ToArray();
                }

                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    RealitBuildContent realitProfile = Resources.Load<RealitBuildContent>("RealitBuildContent");
                    if (realitProfile == null)
                    {
                        realitProfile = RealitBuildContent.CreateInstance<RealitBuildContent>();
                        AssetDatabase.CreateAsset(realitProfile, "Assets/Resources/RealitBuildContent.asset");
                        Debug.Log("Realit Build Content created");
                    }

                    realitProfile.Scenes = buildProfile.scenes;
                    EditorUtility.SetDirty(realitProfile);

                    AssetDatabase.SaveAssetIfDirty(realitProfile);
                }
            }


            GUI.enabled = buildProfile != null && scenes.Count > 0 && scenes.All(ctx => ctx.Value);
            
            if (GUILayout.Button("Send to build settings"))
            {
                List<EditorBuildSettingsScene> orderedScenes = new List<EditorBuildSettingsScene>();

                var editorScenes = EditorBuildSettings.scenes;
                for (int i = 0; i < editorScenes.Length; i++)
                {

                    EditorBuildSettingsScene editorBuildSettingsScene = editorScenes[i];
                    var fileName = Path.GetFileNameWithoutExtension(editorBuildSettingsScene.path);

                    if(fileName == "MainMenu")
                    {
                        editorBuildSettingsScene.enabled = hasMainMenu;
                        orderedScenes.Insert(0, editorBuildSettingsScene);
                    }
                    else
                    {
                        bool enabled = scenes.Any(ctx => ctx.Key.SceneName == Path.GetFileNameWithoutExtension(editorScenes[i].path));
                        editorBuildSettingsScene.enabled = enabled;
                        if(enabled)
                        {
                            if(orderedScenes.Count > 1)
                                orderedScenes.Insert(1, editorBuildSettingsScene);
                            else
                                orderedScenes.Add(editorBuildSettingsScene);

                        }
                        else
                            orderedScenes.Add(editorBuildSettingsScene);
                    }
                }

                EditorBuildSettings.scenes = orderedScenes.ToArray();
            }


            //if(GUILayout.Button("Build to webgl"))
            //    BuildDualWebGL();

            GUI.enabled = true;
        }

        private void SetValues()
        {
            var editorBuildScenes = EditorBuildSettings.scenes;
            buildName = buildProfile.buildName;

            scenes.Clear();

            for (int i = 0; i < buildProfile.scenes.Length; i++)
            {
                RealitScene sceneProfile = buildProfile.scenes[i];
                bool valid = false;

                for (int j = 0; j < editorBuildScenes.Length; j++)
                {
                    if (Path.GetFileNameWithoutExtension(editorBuildScenes[j].path) == sceneProfile.SceneName)
                        valid = true;
                }
                if(!scenes.ContainsKey(sceneProfile))
                    scenes.Add(sceneProfile, valid);
            }
        }

        public static bool IsSymbolDefined(string targetDefineSymbol)
        {
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var defineSymbolsArray = defineSymbols.Split(';');
            for (var i = 0; i < defineSymbolsArray.Length; i++)
            {
                var defineSymbol = defineSymbolsArray[i];
                var trimmedDefineSymbol = defineSymbol.Trim();
                if (trimmedDefineSymbol == targetDefineSymbol)
                {
                    return true;
                }
            }

            return false;
        }

        public static void UpdateSymbol(string targetDefineSymbol, bool value)
        {
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var defineSymbolsArray = defineSymbols.Split(';');
            var newDefineSymbols = string.Empty;
            var isDefined = false;
            for (var i = 0; i < defineSymbolsArray.Length; i++)
            {
                var defineSymbol = defineSymbolsArray[i];
                var trimmedDefineSymbol = defineSymbol.Trim();
                if (trimmedDefineSymbol == targetDefineSymbol)
                {
                    if (!value)
                    {
                        continue;
                    }

                    isDefined = true;
                }

                newDefineSymbols += string.Format("{0};", trimmedDefineSymbol);
            }

            if (value && !isDefined)
            {
                newDefineSymbols += string.Format("{0};", targetDefineSymbol);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefineSymbols);
        }

        //This creates a menu item to trigger the dual builds https://docs.unity3d.com/ScriptReference/MenuItem.html 

        //[MenuItem("Game Build Menu/Dual Build")]
        public void BuildDualWebGL()
        {
            //This builds the player twice: a build with desktop-specific texture settings (WebGL_Build) as well as mobile-specific texture settings (WebGL_Mobile), and combines the necessary files into one directory (WebGL_Build)

            string dualBuildPath = "WebGLBuilds";
            string desktopBuildName = "WebGL_Build";
            string mobileBuildName = "WebGL_Mobile";

            string desktopPath = Path.Combine(dualBuildPath, desktopBuildName);
            string mobilePath = Path.Combine(dualBuildPath, mobileBuildName);
            var scenes = EditorBuildSettings.scenes;

            EditorUserBuildSettings.webGLBuildSubtarget = WebGLTextureSubtarget.DXT;
            BuildPlayerOptions first = new BuildPlayerOptions()
            {
                scenes = scenes.Select(ctx => ctx.path).ToArray(),
                target = BuildTarget.WebGL,
                subtarget = (int)WebGLTextureSubtarget.ASTC,
            };
            BuildPlayerOptions second = new BuildPlayerOptions()
            {
                scenes = scenes.Select(ctx => ctx.path).ToArray(),
                target = BuildTarget.WebGL,
                subtarget = (int)WebGLTextureSubtarget.ETC2,
                options = BuildOptions.AutoRunPlayer
            };

            EditorUserBuildSettings.webGLBuildSubtarget = WebGLTextureSubtarget.ETC2;
            BuildPipeline.BuildPlayer(first);

            EditorUserBuildSettings.webGLBuildSubtarget = WebGLTextureSubtarget.ASTC;
            BuildPipeline.BuildPlayer(second);

            // Copy the mobile.data file to the desktop build directory to consolidate them both
            FileUtil.CopyFileOrDirectory(Path.Combine(mobilePath, "Build", mobileBuildName + ".data"), Path.Combine(desktopPath, "Build", mobileBuildName + ".data"));
        }
    }
}