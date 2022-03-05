/*
 * FileName:    AssetBundleUrlLoader
 * Author:      Wings
 * CreateTime:  2021_11_23
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GenOcean.Assetbundle
{
    /// <summary>
    /// 表明一个Loader的当前状态
    /// </summary>
    public enum AssetBundleLoaderState
    {
        /// <summary>
        /// 未加载
        /// </summary>
        UNLOADED = 0x00,

        /// <summary>
        /// 已加载
        /// </summary>
        LOADED = 0x01,

        /// <summary>
        /// 加载失败
        /// </summary>
        LOAD_FAIL = 0x02,

        /// <summary>
        /// 正在加载
        /// </summary>
        LOADING = 0x03,
    }

    public interface IAssetbundleLoader
    {
        /// <summary>
        /// 开始加载Assetbundle
        /// </summary>
        /// <param name="ab_param"></param>
        /// <returns></returns>
        public IEnumerator StartLoadAssetbundle(string ab_param);

        /// <summary>
        /// 获取Assetbundle结果
        /// </summary>
        /// <returns></returns>
        public AssetBundle GetContent();
    }

    public abstract class AssetBundleLoader : IAssetbundleLoader
    {
        protected AssetBundle _MainAssetBundle;

        public AssetBundle GetContent()
        {
            return _MainAssetBundle;
        }

        public IEnumerator StartLoadAssetbundle(string ab_param)
        {
            yield return null;
        }
    }


    /// <summary>
    /// 网络Assetbundle加载类
    /// </summary>
    public class AssetBundleUrlLoader : MonoBehaviour
    {
        #region Protected Fields

        protected UnityWebRequest _AssetbundleRequest = null;

        #endregion --Protected Fields

        #region Public Fields

        /// <summary>
        /// 加载资源包的路径
        /// </summary>
        public string AssetBundleUrl;

        public AssetBundleLoaderState State = AssetBundleLoaderState.UNLOADED;

        /// <summary>
        /// 加载完成的bundle
        /// </summary>
        public AssetBundle LoadedBundle;

        public bool IsDownloading
        {
            get
            {
                if (_AssetbundleRequest != null)
                {
                    return !_AssetbundleRequest.isDone;
                }

                return false;
            }
        }

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        public virtual void Load(string url = null)
        {
            if (!string.IsNullOrEmpty(url))
            {
                AssetBundleUrl = url;
            }

            LoadSelfAssetbundle();
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        /// <summary>
        /// 加载自身的 Assetbundle 资源
        /// </summary>
        protected virtual bool LoadSelfAssetbundle()
        {
            if (string.IsNullOrEmpty(AssetBundleUrl))
            {
                Debug.LogError($"{name} try load null AssetBundleUrl");
                return false;
            }
#if USE_ABCON
            AssetBundleUrlController.GetAssetBundle(AssetBundleUrl, OnLoadSuccess,OnLoadFail);
#else
            StartCoroutine(DownloadAssetBundle(AssetBundleUrl));
#endif

            return true;
        }

#if !USE_ABCON
        protected virtual IEnumerator DownloadAssetBundle(string uri)
        {
            State = AssetBundleLoaderState.LOADING;
            _AssetbundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(uri);
            yield return _AssetbundleRequest.SendWebRequest();

            // 处理结果
            switch (_AssetbundleRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError($"{name} download url({uri}) fail.(Net error).");
                    State = AssetBundleLoaderState.LOAD_FAIL;
                    break;
                case UnityWebRequest.Result.Success:
                    LoadedBundle = DownloadHandlerAssetBundle.GetContent(_AssetbundleRequest);
                    State = AssetBundleLoaderState.LOADED;
                    OnLoadSuccess();
                    break;
                default:
                    Debug.LogError($"{name} download url({uri}) fail.({_AssetbundleRequest.error}).");
                    State = AssetBundleLoaderState.LOAD_FAIL;
                    break;
            }

            _AssetbundleRequest = null;
        }
#endif

        /// <summary>
        /// Assetbundle 加载成功时执行的操作
        /// </summary>
        protected virtual void OnLoadSuccess()
        {
        }

#if USE_ABCON
        protected virtual void OnLoadSuccess(AssetBundle ab)
        {
             LoadedBundle = ab;
             State = AssetBundleLoaderState.LOADED;
             OnLoadSuccess();
        }

        protected virtual void OnLoadFail(string url)
        {
            State = AssetBundleLoaderState.LOAD_FAIL;
        }
#endif

        #endregion --Protected Methods

        #region Unity Methods

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void OnDestroy()
        {

        }
#endregion --Unity Methods
    }
}