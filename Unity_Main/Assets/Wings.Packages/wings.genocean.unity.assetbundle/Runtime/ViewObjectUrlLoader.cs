/*
 * FileName:    ViewObjectUrlLoader
 * Author:      Wings
 * CreateTime:  2021_11_23
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenOcean.Assetbundle
{
    public class ViewObjectUrlLoader : AssetBundleUrlLoader
    {
        #region Protected Fields
        #endregion --Protected Fields

        #region Public Fields
        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods
        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        protected override void OnLoadSuccess()
        {
            base.OnLoadSuccess();
            Object obj = LoadedBundle.LoadAsset<Object>("Main");
            if(obj)
            {
                GameObject go = (GameObject)Instantiate(obj);
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.rotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }
        }

        #endregion --Protected Methods

        #region Unity Methods

        protected virtual void OnBecameVisible()
        {
#if UNITY_EDITOR
            Debug.Log($"{name} OnBecameVisible");
#endif
            if(!string.IsNullOrEmpty(AssetBundleUrl) && State == AssetBundleLoaderState.UNLOADED)
            {
#if UNITY_EDITOR
                Debug.Log($"{name} LoadSelfAssetbundle");
#endif
                LoadSelfAssetbundle();
            }
        }

        #endregion --Unity Methods
    }
}