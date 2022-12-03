using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Wings.Unity.Editor
{
    public class PrefabUsageInfo
    {
        public GameObject Prefab;
        public int Usage;
    }

    public class PrefabUsageCountEditorWindow:EditorWindow
    {
        #region Instance
        protected static PrefabUsageCountEditorWindow _WIN;
        public PrefabUsageCountEditorWindow()
        {
            titleContent = new GUIContent("Prefab Usage");
        }

        [MenuItem("Wings/Anlysis/Perfab Usage")]
        public static void OpenWindow()
        {
            if (_WIN)
            {
                _WIN.Close();
                EditorWindow.DestroyImmediate(_WIN);
                _WIN = null;
            }

            if (Application.isPlaying)
            {
                return;
            }

            _WIN = (PrefabUsageCountEditorWindow)EditorWindow.GetWindow(typeof(PrefabUsageCountEditorWindow));
            _WIN.Show();
        }
        #endregion


        #region GUI

        protected List<PrefabUsageInfo> _PrefabUsageInfoResult = new List<PrefabUsageInfo>();
        protected Vector2 _ResultPos;

        protected virtual void OnGUI()
        {
            GUILayout.Label($"Select {Selection.objects.Length} objects.");
            //if(GUILayout.Button("Export To"))
            //{
            //    string fp = EditorUtility.SaveFilePanel("Save Anlysis CSV Data","", "Prefab_Usage_Count", ".csv");
            //    if(!string.IsNullOrEmpty(fp))
            //    {
            //        ExportSelectionData(fp);
            //        EditorUtility.DisplayDialog("Export", "Prefab usage export finish!", "OK");
            //    }

            //}

            if (GUILayout.Button("Find All"))
            {
                _PrefabUsageInfoResult = FindSelectionUsage();
            }

            if(_PrefabUsageInfoResult.Count>0)
            {
                _ResultPos = GUILayout.BeginScrollView(_ResultPos);
                for (int i = 0; i < _PrefabUsageInfoResult.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField(_PrefabUsageInfoResult[i].Prefab, typeof(GameObject), true);
                    }

                    GUILayout.Label(_PrefabUsageInfoResult[i].Usage.ToString());
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }
        }

        #endregion

        protected virtual void ExportSelectionData(string fp)
        {
            FileInfo fi = new FileInfo(fp);
            if(!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

        }

        protected virtual List<PrefabUsageInfo> FindSelectionUsage()
        {
            Dictionary<GameObject, PrefabUsageInfo> dictInfo = new Dictionary<GameObject, PrefabUsageInfo>();

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                GameObject go = Selection.objects[i] as GameObject;
                if (!go || go.scene.rootCount != 0)
                {
                    go = PrefabUtility.GetCorrespondingObjectFromSource(go);
                    if (!go || go.scene.rootCount != 0)
                    {
                        continue;
                    }
                }

                PrefabUsageInfo info = new PrefabUsageInfo();
                info.Prefab = go;
                info.Usage = PrefabUtility.FindAllInstancesOfPrefab(go).Length;

                if(!dictInfo.ContainsKey(info.Prefab))
                {
                    dictInfo.Add(info.Prefab, info);
                }
            }


            return dictInfo.Values.ToList();
        }
    }
}
