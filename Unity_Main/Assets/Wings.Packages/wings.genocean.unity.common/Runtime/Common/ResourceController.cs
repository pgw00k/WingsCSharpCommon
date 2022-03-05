/*
 * FileName:    ResourceController
 * Author:      Wings
 * CreateTime:  2021_12_30
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenOcean.Common
{
    public interface IResource
    {
        public T Load<T>(int rid);
    }

    /// <summary>
    /// 资源加载管理器的基类
    /// </summary>
    public class ResourceController : MonoBehaviour
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
