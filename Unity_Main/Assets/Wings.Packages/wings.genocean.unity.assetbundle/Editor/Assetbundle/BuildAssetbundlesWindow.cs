/*
 * FileName:    BuildAssetbundlesWindow
 * Author:      Wings
 * CreateTime:  2021_11_24
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GenOcean.Unity.Editor.Assetbundle
{
    public class BuildAssetbundlesWindow : SingleEditorWindow<BuildAssetbundlesWindow>
    {
        #region Protected Fields

        protected static string SettingSaveFile = "Assets/Editor/BuildAssetbundlesWindow.asset";

        #endregion --Protected Fields

        #region Public Fields

        public BuildTarget Target = BuildTarget.WebGL;
        public BuildAssetBundleOptions BuildABOption = BuildAssetBundleOptions.None;

        public string AssetbundleRootFolder = "Assets";
        public string AssetbundleOutputFolder = "Assets";

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        [MenuItem("Wings/Assetbundle/Build Assetbundle")]
        public static void Open()
        {
            BuildAssetbundlesWindow.OpenWindow();
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        protected virtual void LoadSetting(string fileFull)
        {
            EditorSaveFile t = AssetDatabase.LoadAssetAtPath<EditorSaveFile>(fileFull);
            if(t)
            {
                AssetbundleRootFolder = t["AssetbundleRootFolder"];
                AssetbundleOutputFolder = t["AssetbundleOutputFolder"];
            }
        }

        protected virtual void SaveSetting(string fileFull)
        {
            EditorSaveFile t = AssetDatabase.LoadAssetAtPath<EditorSaveFile>(fileFull);
            if (t)
            {
                t["AssetbundleRootFolder"] = AssetbundleRootFolder;
                t["AssetbundleOutputFolder"] = AssetbundleOutputFolder;
                //AssetDatabase.CreateAsset(t, fileFull);
                EditorUtility.SetDirty(t);
            }
            
        }

        #endregion --Protected Methods

        #region Unity Methods

        protected override void Awake()
        {
            LoadSetting(SettingSaveFile);
        }

        protected virtual void OnGUI()
        {
            GUILayout.Label("Assetbundle Tools");

            GUILayout.BeginHorizontal();
            Target = (BuildTarget)EditorGUILayout.EnumPopup(Target);
            BuildABOption = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup(BuildABOption);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            AssetbundleRootFolder = EditorGUILayout.TextField(AssetbundleRootFolder);
            if (GUILayout.Button("Input"))
            {
                AssetbundleRootFolder = EditorUtility.OpenFolderPanel("Assetbundles Root", AssetbundleRootFolder, "");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            AssetbundleOutputFolder = EditorGUILayout.TextField(AssetbundleOutputFolder);
            if (GUILayout.Button("Output"))
            {
                AssetbundleOutputFolder = EditorUtility.OpenFolderPanel("Output", AssetbundleOutputFolder, "");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Names"))
            {

            }

            if (GUILayout.Button("Set Names"))
            {
                BuildAssetbundleTools.SetNames(AssetbundleRootFolder);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Build"))
            {
                BuildAssetbundleTools.BuildAssetbundlesByDirectory(AssetbundleRootFolder, AssetbundleOutputFolder, BuildABOption, Target);
            }

        }


        protected override void OnDestroy()
        {
            SaveSetting(SettingSaveFile);
        }

        #endregion --Unity Methods
    }
}
