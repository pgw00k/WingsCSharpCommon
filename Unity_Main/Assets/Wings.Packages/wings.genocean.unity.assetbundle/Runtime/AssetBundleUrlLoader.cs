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
    /// ����һ��Loader�ĵ�ǰ״̬
    /// </summary>
    public enum AssetBundleLoaderState
    {
        /// <summary>
        /// δ����
        /// </summary>
        UNLOADED = 0x00,

        /// <summary>
        /// �Ѽ���
        /// </summary>
        LOADED = 0x01,

        /// <summary>
        /// ����ʧ��
        /// </summary>
        LOAD_FAIL = 0x02,

        /// <summary>
        /// ���ڼ���
        /// </summary>
        LOADING = 0x03,
    }

    public interface IAssetbundleLoader
    {
        /// <summary>
        /// ��ʼ����Assetbundle
        /// </summary>
        /// <param name="ab_param"></param>
        /// <returns></returns>
        public IEnumerator StartLoadAssetbundle(string ab_param);

        /// <summary>
        /// ��ȡAssetbundle���
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
    /// ����Assetbundle������
    /// </summary>
    public class AssetBundleUrlLoader : MonoBehaviour
    {
        #region Protected Fields

        protected UnityWebRequest _AssetbundleRequest = null;

        #endregion --Protected Fields

        #region Public Fields

        /// <summary>
        /// ������Դ����·��
        /// </summary>
        public string AssetBundleUrl;

        public AssetBundleLoaderState State = AssetBundleLoaderState.UNLOADED;

        /// <summary>
        /// ������ɵ�bundle
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
        /// ��������� Assetbundle ��Դ
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

            // ������
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
        /// Assetbundle ���سɹ�ʱִ�еĲ���
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