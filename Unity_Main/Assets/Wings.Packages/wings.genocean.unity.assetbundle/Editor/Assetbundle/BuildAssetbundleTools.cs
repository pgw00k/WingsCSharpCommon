using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GenOcean.Unity.Editor.Assetbundle
{
    public class BuildAssetbundleTools
    {
        /// <summary>
        /// 根据文件路径设置 AB 包标签名
        /// </summary>
        /// <param name="root"></param>
        public static void SetNames(string root)
        {
        }

        public static void BuildAssetbundlesByNames(string input,string output,BuildAssetBundleOptions op,BuildTarget tar)
        {
            BuildPipeline.BuildAssetBundles(output, op, tar);
        }

        public static void BuildAssetbundlesByDirectory(string input,string output, BuildAssetBundleOptions op, BuildTarget tar)
        {
            string foldPath = FileUtil.GetProjectRelativePath(input);

            DirectoryInfo di = new DirectoryInfo(input);
            Dictionary<string, List<string>> dicts = new Dictionary<string, List<string>>();

            string[] sub = new string[] { FileUtil.GetProjectRelativePath(input) };
            string[] fies = AssetDatabase.FindAssets("*", sub);

            for (int i = 0; i < fies.Length; i++)
            {
                string guid = fies[i];
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if(AssetDatabase.IsValidFolder(assetPath))
                {
                    continue;
                }
                FileInfo fi = new FileInfo(assetPath);
                if(!dicts.ContainsKey(fi.Directory.FullName))
                {
                    dicts.Add(fi.Directory.FullName, new List<string>());
                }

                dicts[fi.Directory.FullName].Add(assetPath);
            }

            List<AssetBundleBuild> abbs = new List<AssetBundleBuild>();
            int checkCount = 0,checkMax = 10;
            List<string> checkName = new List<string>();
            foreach(var kp in dicts)
            {
                DirectoryInfo cdi = new DirectoryInfo(kp.Key);
                string cname = cdi.Name;
                CheckDirectoryName:
                if (checkName.Contains(cname) && checkCount < checkMax)
                {
                    checkCount++;
                    cname += "_1";
                    goto CheckDirectoryName;
                }

                if (checkName.Contains(cname))
                {
                    Debug.LogWarning($"{cdi.FullName} can not build as assetbunld.");
                    continue;
                }

                checkName.Add(cname);
                AssetBundleBuild abb = new AssetBundleBuild()
                {
                    assetBundleName = cname,
                    assetBundleVariant = "unityweb",
                    assetNames = kp.Value.ToArray()
                };

                abbs.Add(abb);
            }

            BuildPipeline.BuildAssetBundles(output,abbs.ToArray(), op, tar);

            Debug.Log($"{abbs.Count} asset folder build to {output}");
        }
    }
}
