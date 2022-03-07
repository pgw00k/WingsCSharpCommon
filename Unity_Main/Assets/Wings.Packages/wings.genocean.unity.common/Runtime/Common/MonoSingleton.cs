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
        protected static Dictionary<Type, IMonoSingleObject> _SingleObjects = new Dictionary<Type, IMonoSingleObject>();

        public static T GetInstance<T>()
            where T : IMonoSingleObject
        {
            T obj = default(T);
            if (_SingleObjects.TryGetValue(typeof(T),out IMonoSingleObject mono))
            {
                obj = (T)mono;
            }

            return obj;
        }

        public static void SetInstance(object obj,Type type)
        {
            if (!_SingleObjects.TryGetValue(type, out IMonoSingleObject mono))
            {
                _SingleObjects.Add(type, (IMonoSingleObject)obj);
            }
            else
            {
                IMonoSingleObject oldIns = mono;
            }
        }

        public static void ReleaseInstance<T>()
        {
            if (_SingleObjects.TryGetValue(typeof(T), out IMonoSingleObject mono))
            {
                _SingleObjects.Remove(typeof(T));
            }
        }
    }

    /// <summary>
    /// Mono 单例，继承自 MonoBehaviour
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour, IMonoSingleObject where T : MonoBehaviour
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
                    MonoSingleTonManager.SetInstance(this,this.GetType());
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
