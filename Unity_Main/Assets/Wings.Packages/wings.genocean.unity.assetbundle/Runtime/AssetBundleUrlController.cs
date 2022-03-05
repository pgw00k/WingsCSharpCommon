/*
 * FileName:    AssetBundleUrlController
 * Author:      Wings
 * CreateTime:  2021_11_24
 * 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GenOcean.Assetbundle
{
    /// <summary>
    /// 管理所有的热加载资源包，单例
    /// <para>主要用于重复资源减少请求</para>
    /// </summary>
    public class AssetBundleUrlController : MonoBehaviour
    {

        public class AssetBundleLoadRequest
        {
            public string Url;
            public Stack<Action<AssetBundle>> OnSuccess;
            public Stack<Action<string>> OnFail;
            public AssetBundle Bundle;
            public AssetBundleLoaderState State = AssetBundleLoaderState.UNLOADED;

            public void Clear()
            {
                OnSuccess.Clear();
                OnFail.Clear();
            }
        }

        #region Protected Fields

        protected static AssetBundleUrlController _Instance = null;
        protected static object _Locker = new object();
        protected static Dictionary<string, AssetBundleLoadRequest> _Bundles = new Dictionary<string, AssetBundleLoadRequest>();
        protected static Dictionary<string, AssetBundleLoadRequest> _Requestes = new Dictionary<string, AssetBundleLoadRequest>();

        protected List<string> _DownloadOver = new List<string>();

        #endregion --Protected Fields

        #region Public Fields
        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        public static AssetBundleLoaderState GetAssetBundle(string url,Action<AssetBundle> onSuccess = null, Action<string> onFail = null)
        {
            AssetBundleLoadRequest req = null;
            if(_Bundles.TryGetValue(url,out req))
            {
                onSuccess.Invoke(req.Bundle);
                return AssetBundleLoaderState.LOADED;
            }

            lock (_Locker)
            {
                if (!_Requestes.TryGetValue(url, out req))
                {
                    req = new AssetBundleLoadRequest();
                    req.Url = url;
                    req.OnSuccess = new Stack<Action<AssetBundle>>();
                    req.OnFail = new Stack<Action<string>>();

                    _Requestes.Add(url, req);
                }

                if (onSuccess != null)
                {
                    req.OnSuccess.Push(onSuccess);
                }

                if (onFail != null)
                {
                    req.OnFail.Push(onFail);
                }

                return req.State;
            }
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        /// <summary>
        /// 检测所有需要下载 ab 包的状态
        /// </summary>
        protected virtual void CheckAssetbundles()
        {
            lock (_Locker)
            {
                _DownloadOver.Clear();
                foreach (var kp in _Requestes)
                {
                    switch(kp.Value.State)
                    {
                        case AssetBundleLoaderState.UNLOADED:
                            StartCoroutine(DownloadAssetbundle(kp.Value));
                            break;
                        case AssetBundleLoaderState.LOADED:
                        case AssetBundleLoaderState.LOAD_FAIL:
                            _DownloadOver.Add(kp.Key);
                            break;
                    }
                }

                foreach(string key in _DownloadOver)
                {
                    AssetBundleLoadRequest req = _Requestes[key];
                    _Requestes.Remove(key);
                    _Bundles.Add(req.Url, req);
                    switch (req.State)
                    {
                        case AssetBundleLoaderState.LOADED:
                            DispatchSuccess(req);
                            break;
                        case AssetBundleLoaderState.LOAD_FAIL:
                            DispatchFail(req);
                            break;
                    }

                    req.Clear();
                }

                _DownloadOver.Clear();
            }
        }

        /// <summary>
        /// 分发下载成功事件
        /// </summary>
        /// <param name="req"></param>
        protected virtual void DispatchSuccess(AssetBundleLoadRequest req)
        {
            for(int i =0;i<req.OnSuccess.Count;i++)
            {
                Action<AssetBundle> act = req.OnSuccess.Pop();
                try
                {
                    act.Invoke(req.Bundle);
                }catch(Exception err)
                {
#if UNITY_EDITOR || ERROR_LOG

                    Debug.LogError($"{req.Url} onSuccess({act.Target}.{act.Method}) invoke fail.\n{err.Message}");

#endif
                }
            }
        }

        /// <summary>
        /// 分发下载失败事件
        /// </summary>
        /// <param name="req"></param>
        protected virtual void DispatchFail(AssetBundleLoadRequest req)
        {
            for (int i = 0; i < req.OnSuccess.Count; i++)
            {
                Action<string> act = req.OnFail.Pop();
                try
                {
                    act.Invoke(req.Url);
                }
                catch (Exception e)
                {
#if UNITY_EDITOR || ERROR_LOG

                    Debug.LogError($"{req.Url} onSuccess({act.Target}.{act.Method}) invoke fail.\n{e.Message}");

#endif
                }
            }
        }

        protected virtual IEnumerator DownloadAssetbundle(AssetBundleLoadRequest req)
        {
            req.State = AssetBundleLoaderState.LOADING;
            UnityWebRequest _AssetbundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(req.Url);
            yield return _AssetbundleRequest.SendWebRequest();

            // 处理结果
            switch (_AssetbundleRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError($"{name} download url({req.Url}) fail.(Net error).");
                    req.State = AssetBundleLoaderState.LOAD_FAIL;
                    break;
                case UnityWebRequest.Result.Success:
                    req.Bundle = DownloadHandlerAssetBundle.GetContent(_AssetbundleRequest);
                    req.State = AssetBundleLoaderState.LOADED;
                    break;
                default:
                    Debug.LogError($"{name} download url({req.Url}) fail.({_AssetbundleRequest.error}).");
                    req.State = AssetBundleLoaderState.LOAD_FAIL;
                    break;
            }

            _AssetbundleRequest = null;
        }

        #endregion --Protected Methods

        #region Unity Methods

        protected virtual void Awake()
        {
            if(!_Instance)
            {
                _Instance = this;
            }
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            CheckAssetbundles();
        }

        protected virtual void OnDestroy()
        {

        }
        #endregion --Unity Methods
    }
}
