/*
 * FileName:    EventMonoComponent
 * Author:      Wings
 * CreateTime:  2022_01_13
 * 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GenOcean.Common;

namespace GenOcean.GameFrame
{
    public class SingleDefaultEventManager : SingleEventManager
    {
    }

    /// <summary>
    /// EventMono �������̳��� EventMonoComponent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventMonoSingleton<T> : EventMonoComponent where T : EventMonoComponent
    {
        #region Protected Fields

        /// <summary>
        /// �洢������ָ��
        /// </summary>
        protected volatile static T _Instance = null;

        /// <summary>
        /// ���Ա�֤�̰߳�ȫ����
        /// </summary>
        protected static readonly object _LockObject = new object();



        #endregion --Protected Fields

        #region Public Fields


        /// <summary>
        /// �������
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

        public static bool IsInitCompelete
        {
            get
            {
                return Instance && Instance.IsInit;
            }
        }

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        public static void ReInit()
        {
            Instance.Init();
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods
        #endregion --Protected Methods

        #region Unity Methods

        protected override void Awake()
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
                    base.Awake();
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
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

    /// <summary>
    /// �����¼���MONO����
    /// </summary>
    public class EventMonoComponent : MonoBehaviour, IRegsiterEvents
    {
        #region Protected Fields

        protected Dictionary<int, int> _EUIDS = new Dictionary<int, int>();
        protected bool _IsRegister = false;
        protected bool _IsInit = false;

        #endregion --Protected Fields

        #region Public Fields

        public Dictionary<int, int> EventIDs
        {
            get
            {
                return _EUIDS;
            }
            set
            {
                _EUIDS = value;
            }
        }

        public bool IsRegister
        {
            get
            {
                return _IsRegister;
            }
            set
            {
                _IsRegister = value;
            }
        }

        public bool IsInit
        {
            get
            {
                return _IsInit;
            }
        }

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public virtual void Init()
        {
            if(!_IsRegister)
            {
                _IsRegister = true;
                RegsiterEvents();
            }
        }

        /// <summary>
        /// ע������¼�
        /// </summary>
        public virtual void RegsiterEvents()
        {        
        }

        /// <summary>
        /// ȡ�������¼�
        /// </summary>
        public virtual void UnregsiterEvents()
        {
            if(_IsRegister && EventIDs.Count > 0)
            {
                _IsRegister = false;
                foreach (var k in EventIDs.Keys)
                {
                    SingleDefaultEventManager.ReleaseEventCallback(k,EventIDs[k]);
                }

                EventIDs.Clear();
            }
        }

        /// <summary>
        /// ע�ᵥ�������¼�
        /// <para>ͬʱ���¼�ڼ������У��ڶ�������ʱ��ȡ������</para>
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="cb"></param>
        /// <param name="isSwallow"></param>
        /// <param name="invokeCount"></param>
        public virtual void RegsiterEvent(int eid, Action<object> cb, bool isSwallow = false, int invokeCount = 1)
        {
            int uid = SingleDefaultEventManager.RegisterEventCallback(eid, cb, isSwallow, invokeCount);
            EventIDs.Add(eid, uid);
        }

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
            if (!_IsInit)
            {
                _IsInit = true;
                Init();
            }
        }

        protected virtual void OnDestroy()
        {
            UnregsiterEvents();
        }

        #endregion --Unity Methods
    }
}
