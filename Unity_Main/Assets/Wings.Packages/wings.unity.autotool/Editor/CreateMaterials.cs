using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateMaterials : EditorWindow
{
    #region Bascis Menu Setting
    private static CreateMaterials _Instance = null;
    [MenuItem("Wings/Auto Tool/CreateMaterials")]
    public static void ShowWin()
    {
        if (_Instance)
        {
            _Instance.Close();
        }

        _Instance = EditorWindow.CreateWindow<CreateMaterials>("CreateMaterials");
        _Instance.Show();
    }
    #endregion

    #region GUI

    private string _TargetFolderFullPath;

    private GameObject model;

    private List<Material> mats = new List<Material>();

    private string[] shaders = new string[]
    {
        "Standard",
        "Standard (Specular setup)"
    };

    private int _main_Shader = 1;

    protected virtual void OnGUI()
    {
        if (GUILayout.Button("Test"))
        {
            Material m = GetMaterialAsset("Assets/AutoSetTextures/AAA.mat");

            string rel = @"F:/Unity_Projects/New Unity Project/Assets/AutoSetTextures/AAA.mat";

            Texture tex = AssetDatabase.LoadAssetAtPath<Texture2D>(@"Assets\AutoSetTextures\Cat16M\textures\Cat16MEngineFEOFuelWSAirFH_albedo.tga");

            Debug.Log(Selection.activeObject.GetType());
            Debug.Log(AssetDatabase.GetAssetPath(Selection.activeObject));

            if (!tex)
            {
                Debug.Log("Null Texture");
            }

            m.SetTexture("_MainTex", tex) ;
        }

        model = (GameObject)EditorGUILayout.ObjectField("Model", model, typeof(GameObject), false);

        _main_Shader = EditorGUILayout.Popup("Shader", _main_Shader, shaders);

        GUILayout.BeginHorizontal();
        _TargetFolderFullPath = EditorGUILayout.TextField("Working", _TargetFolderFullPath);
        if (GUILayout.Button("Browser"))
        {
            DirectoryInfo di = new DirectoryInfo(Application.dataPath);
            _TargetFolderFullPath = EditorUtility.SaveFolderPanel("Select working folder", di.Parent.FullName, "Working");
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("SetMaterials"))
        {
            if (string.IsNullOrEmpty(_TargetFolderFullPath))
            {
                EditorUtility.DisplayDialog("Error", "未设置贴图文件夹！", "OK");
                return;
            }

            if (!model)
            {
                EditorUtility.DisplayDialog("Error", "未设置目标模型！", "OK");
                return;
            }

            ModelImporter im = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(model)) as ModelImporter;

            if (im)
            {
                Dictionary<AssetImporter.SourceAssetIdentifier, Object> dict = im.GetExternalObjectMap();
                List<AssetImporter.SourceAssetIdentifier> keys = dict.Keys.ToList();

                // 清空关联关系，重新设置
                foreach (AssetImporter.SourceAssetIdentifier k in keys)
                {
                    if(k.type == typeof(Material) && !dict[k])
                    {
                        im.RemoveRemap(k);
                    }
                }

                im.SaveAndReimport();
            }

            foreach (Renderer r in model.GetComponentsInChildren<Renderer>())
            {
                Material[] ms = r.sharedMaterials;
                foreach (Material m in ms)
                {
                    if (mats.Contains(m))
                    {
                        continue;
                    }

                    Debug.Log(m.name);
                    mats.Add(m);
                }
            }


            List<FileInfo> files = new List<FileInfo>();
            GetFiles(_TargetFolderFullPath, ref files);

            FileInfo fmodel = new FileInfo(AssetDatabase.GetAssetPath(model));
            string texFolder = fmodel.Directory.FullName + "/textures/";
            string matFolder = fmodel.Directory.FullName + "/materials/";

            texFolder = RelativePath(Application.dataPath, texFolder);
            matFolder = RelativePath(Application.dataPath,matFolder);

            foreach (Material m in mats)
            {
                Material mi = GetMaterialAsset(matFolder + m.name + ".mat", shaders[_main_Shader]);

                if(mi)
                {
                    SetMaterial(mi, texFolder, ref files);
                }
                

                if(im && im.GetExternalObjectMap().ContainsValue(mi))
                {
                    im.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), mi.name), mi);
                }
                
            }

            if (im)
            {
                im.SaveAndReimport();
            }
            

            AssetDatabase.SaveAssets();
        }
    }

    private void SetMaterial(Material m,string texFolder,ref List<FileInfo> files)
    {
        string mainName = m.name;
        if(mainName.ToLower().EndsWith("_albedo"))
        {
            mainName = mainName.Remove(mainName.Length - 7, 7);
        }

        if(mainName.ToLower().EndsWith("_m"))
        {
            mainName = mainName.Remove(mainName.Length - 2, 2);
        }

        string texPath = texFolder + mainName + "_albedo.tga";
        Texture tex = GetTextureAsset(texPath, ref files);
        if(tex)
        {
            m.SetTexture("_MainTex", tex);
        }

        texPath = texFolder + mainName + "_metallic.tga";
        tex = GetTextureAsset(texPath, ref files);
        if (tex)
        {
            m.SetTexture("_MetallicGlossMap", tex);
            m.SetTexture("_SpecGlossMap", tex);
        }

        texPath = texFolder + mainName + "_normals.tga";
        tex = GetTextureAsset(texPath, ref files);
        if (tex)
        {
            m.SetTexture("_BumpMap", tex);
        }

        texPath = texFolder + mainName + "_occlusion.tga";
        tex = GetTextureAsset(texPath, ref files);
        if (tex)
        {
            m.SetTexture("_OcclusionMap", tex);
        }
    }


    private void GetFiles(string dirs, ref List<FileInfo> list)
    {
        //绑定到指定的文件夹目录
        DirectoryInfo dir = new DirectoryInfo(dirs);
        //检索表示当前目录的文件和子目录
        //FileSystemInfo[] fsinfos = dir.GetFileSystemInfos("*.tga", SearchOption.AllDirectories);
        FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();

        //遍历检索的文件和子目录
        foreach (FileSystemInfo fsinfo in fsinfos)
        {
            //判断是否为空文件夹　　
            if (fsinfo is DirectoryInfo)
            {
                //递归调用
                GetFiles(fsinfo.FullName, ref list);
            }
            else
            {
                //将得到的文件全路径放入到集合中
                list.Add(new FileInfo(fsinfo.FullName));
            }
        }
    }

    #endregion

    private static string RelativePath(string absolutePath, string relativeTo)
    {
        string[] absoluteDirectories = absolutePath.Split('\\','/');
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
        for (index = lastCommonRoot+1; index < absoluteDirectories.Length; index++)
            if (absoluteDirectories[index].Length > 0)
                relativePath+="..\\";

        //Add on the folders
        for (index = lastCommonRoot ; index < relativeDirectories.Length - 1; index++)
            relativePath+=(relativeDirectories[index] + "\\");
        relativePath+=(relativeDirectories[relativeDirectories.Length - 1]);

        return relativePath;
    }

    public static Material GetMaterialAsset(string path, string shader = "Standard")
    {
        Shader sd = Shader.Find(shader);
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (!m)
        {
            FileInfo fi = new FileInfo(path);
            string parent = RelativePath(fi.Directory.FullName, Application.dataPath);
            string parentRoot = RelativePath(fi.Directory.Parent.FullName, Application.dataPath);
            if(!AssetDatabase.IsValidFolder(parent))
            {
                AssetDatabase.CreateFolder(parentRoot, fi.Directory.Name);
            }

            m = new Material(sd);
            AssetDatabase.CreateAsset(m, path);

        }

        m.shader = sd;

        return m;
    }

    public static Texture GetTextureAsset(string path,ref List<FileInfo> list)
    {
        Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(path);
        if (!tex)
        {
            FileInfo fi = new FileInfo(path);
            FileInfo fi2 = list.Find(fii => fii.Name == fi.Name);

            if (fi2 != null)
            {
                string parent = RelativePath(fi.Directory.FullName, Application.dataPath);
                string parentRoot = RelativePath(fi.Directory.Parent.FullName, Application.dataPath);
                if (!AssetDatabase.IsValidFolder(parent))
                {
                    AssetDatabase.CreateFolder(parentRoot, fi.Directory.Name);
                }
                string relpath = RelativePath(Application.dataPath, fi.FullName);
                File.Copy(fi2.FullName, fi.FullName);
                AssetDatabase.ImportAsset(path);
                list.Remove(fi2);
            }
            else
            {
                Debug.Log(string.Format("Can not find {0}", fi.Name));
            }
            tex = AssetDatabase.LoadAssetAtPath<Texture>(path);
        }
        return tex;
    }
}
