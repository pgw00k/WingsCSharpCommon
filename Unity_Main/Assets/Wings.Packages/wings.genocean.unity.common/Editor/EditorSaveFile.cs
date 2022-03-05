/*
 * FileName:    EditorSaveFile
 * Author:      Wings
 * CreateTime:  2021_11_24
 * 
*/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GenOcean.Unity.Editor
{
    public class EditorSaveFile : ScriptableObject
    {
        [Serializable]
        public class KVPair<TKey,TValue>
        {
            public TKey Key;
            public TKey Value;
        };

        #region Protected Fields

        #endregion --Protected Fields

        #region Public Fields

        [SerializeField]
        public List<KVPair<string,string>> MainDict = new List<KVPair<string, string>>();

        public string this[string key]
        {
            get
            {
                return MainDict.Find(p => p.Key == key).Value;
            }

            set
            {
                MainDict.Find(p => p.Key == key).Value = value;
            }
        }

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        [MenuItem("Assets/Create EditorSaveFile/Base")]
        public static void MenuCreateInstance()
        {
            string foldPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!AssetDatabase.IsValidFolder(foldPath))
            {
                FileInfo fi = new FileInfo(foldPath);
                foldPath = foldPath.Replace(fi.Name,"");
            }

            CreateOne(foldPath);
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        protected static bool CreateOne(string foldPath,string fileName = "NewFile")
        {
            int CheckCount = 0;
            int CheckMax = 10;
            CheckFileName:
            string fullPath = Path.Combine(foldPath, fileName + ".asset");
            FileInfo nfi = new FileInfo(fullPath);
            if (File.Exists(nfi.FullName) && CheckCount < CheckMax)
            {
                fileName += "_1";
                CheckCount++;
                goto CheckFileName;
            }

            if (File.Exists(nfi.FullName))
            {
                Debug.Log($"{nfi} is already exists.");
                return false;
            }

            EditorSaveFile t = ScriptableObject.CreateInstance<EditorSaveFile>();
            AssetDatabase.CreateAsset(t, fullPath);

            return true;
        }

        #endregion --Protected Methods

        #region Unity Methods
        #endregion --Unity Methods
    }
}
