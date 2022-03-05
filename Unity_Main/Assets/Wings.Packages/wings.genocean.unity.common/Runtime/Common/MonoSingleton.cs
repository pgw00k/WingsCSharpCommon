/*
 * FileName:    MonoSingleton
 * Author:      Wings
 * CreateTime:  2021_12_11
 * 
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GenOcean.Common
{
    public interface IMonoSingleObject
    {

    }

    public class MonoSingleTonManager
    {
        protected static Dictionary<Type, MonoBehaviour> _SingleObjects = new Dictionary<Type, MonoBehaviour>();

        public static T GetInstance<T>()
            where T : MonoBehaviour
        {
            T obj = null;
            if (_SingleObjects.TryGetValue(typeof(T),out MonoBehaviour mono))
            {
                obj = mono as T;
            }

            return obj;
        }

        public static void SetInstance<T>(T obj)
            where T : MonoBehaviour
        {
            if (!_SingleObjects.TryGetValue(typeof(T), out MonoBehaviour mono))
            {
                
            }else
            {
                T oldIns = mono as T;
            }
        }

        public static void ReleaseInstance<T>()
        {

        }
    }

    /// <summary>
    /// Mono 单例，继承自 MonoBehaviour
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Protected Fields

        /// <summary>
        /// 存储单例的指针
        /// </summary>
        protected volatile static T _Instance = null;

        /// <summary>
        /// 用以保证线程安全的锁
        /// </summary>
        protected static readonly object _LockObject = new object();

        /// <summary>
        /// 单例入口
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (_LockObject)
                {
                    if (_Instance == null)
                    {
                        Debug.LogError($"{typeof(T).Name} get singleton fail.");
                    }
                }
                return _Instance;
            }
        }

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
            lock (_LockObject)
            {
                if (_Instance)
                {
                    Debug.LogError($"{typeof(T).Name} get duplicate singleton.");
                }
                else
                {
                    _Instance = GetComponent<T>();
                }
            }
        }

        protected virtual void OnDestroy()
        {
            lock (_LockObject)
            {
                if (_Instance && _Instance == GetComponent<T>())
                {
                    _Instance = null;
                }
            }
        }
        #endregion --Unity Methods
    }
}
