using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wings.Unity.Editor
{
    public class SetMaterialByNameEditorWindow : EditorWindow
    {
        #region Bascis Menu Setting
        private static SetMaterialByNameEditorWindow _Instance = null;
        [MenuItem("Wings/Auto Tool/Set Material")]
        public static void ShowWin()
        {
            if (_Instance)
            {
                try
                {
                    _Instance.Close();
                }catch(System.Exception err)
                {
                    Debug.LogErrorFormat("SetMaterialByName Close Faild:{0}", err.Message);
                }
            }

            _Instance = EditorWindow.CreateWindow<SetMaterialByNameEditorWindow>("SetMaterialByName");
            _Instance.Show();
        }
        #endregion

        #region GUI

        protected Material _MainMaterial;
        protected bool _IsUseSelectMaterial = false;
        protected bool _IsUseSpecifyTextureFolder = false;
        protected string _TextureFolderPath = "/Tex/";

        protected virtual void OnGUI()
        {

            _TextureFolderPath = EditorGUILayout.TextArea("Texture Path", _TextureFolderPath);

            _IsUseSelectMaterial = EditorGUILayout.Toggle("Use Select Material", _IsUseSelectMaterial);

            if(_IsUseSelectMaterial)
            {
                GUILayout.Label($"Select {Selection.objects.Length} Objects.");
            }else
            {
                _MainMaterial = (Material)EditorGUILayout.ObjectField("MainMaterial", _MainMaterial, typeof(Material), false);
            }


            if(GUILayout.Button("Set"))
            {
                if (_IsUseSelectMaterial)
                {
                    int i = 0;
                    foreach(var obj in Selection.objects)
                    {
                        Material m = obj as Material;
                        if(m)
                        {
                            i++;
                            SetMaterilByName(m);
                        }
                    }
                    EditorUtility.DisplayDialog("Wings Auto Tool", $"Set {i} Material Texture Success!", "OK");
                }
                else
                {
                    SetMaterilByName(_MainMaterial);
                }
            }
        }

        #endregion

        #region Helper Function

        public static string RelativePath(string absolutePath, string relativeTo)
        {
            string[] absoluteDirectories = absolutePath.Split('\\', '/');
            string[] relativeDirectories = relativeTo.Split('\\', '/');

            //Get the shortest of the two paths
            int length = absoluteDirectories.Length < relativeDirectories.Length ? absoluteDirectories.Length : relativeDirectories.Length;

            //Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            //Find common root
            for (index = 0; index < length; index++)
                if (absoluteDirectories[index] == relativeDirectories[index])
                    lastCommonRoot = index;
                else
                    break;

            //If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
            {
                return absolutePath;
            }

            //Build up the relative path
            string relativePath = "";

            //Add on the ..
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++)
                if (absoluteDirectories[index].Length > 0)
                    relativePath += "..\\";

            //Add on the folders
            for (index = lastCommonRoot; index < relativeDirectories.Length - 1; index++)
                relativePath += (relativeDirectories[index] + "\\");
            relativePath += (relativeDirectories[relativeDirectories.Length - 1]);

            return relativePath;
        }

        #endregion

        public Dictionary<string, string[]> MaterialKeys = new Dictionary<string, string[]>() {
            {"_MainTex",new string[]{"_D.tga"} },
            {"_SpecGlossMap",new string[]{"_R.tga"} },
            {"_BumpMap",new string[]{"_N.tga"} },
            {"_MetallicGlossMap",new string[]{"_M.tga"} },
            {"_OcclusionMap",new string[]{"_AO.tga"} },
        };

        public virtual void SetMaterilByName(Material m)
        {
            string fp = AssetDatabase.GetAssetPath(m);

            FileInfo fiTarget = new FileInfo(fp);
            string texFolder = Path.Combine(fiTarget.Directory.Parent.FullName , _TextureFolderPath);

            texFolder = RelativePath(Application.dataPath, texFolder);

            foreach(var kp in MaterialKeys)
            {
                bool isSet = false;
                foreach(var tn in kp.Value)
                {
                    var tp = Path.Combine(texFolder, m.name+ tn);
                    //Debug.Log($"{m.name}.{kp.Key} Try Load {tp}");
                    Texture t = AssetDatabase.LoadAssetAtPath<Texture>(tp);
                    if(t)
                    {
                        m.SetTexture(kp.Key, t);
                        isSet = true;
                        break;
                    }
                }
                if(!isSet)
                {
                    Debug.Log($"{m.name}.{kp.Key} Not Found Texture.");
                }
            }

        }
    }
}
