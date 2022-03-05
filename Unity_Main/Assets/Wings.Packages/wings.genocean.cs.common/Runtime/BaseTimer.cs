/*
 * FileName:    BaseTimer
 * Author:      Wings
 * CreateTime:  2022_01_11
 * 
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace GenOcean.Common
{
    public interface IBaseTimer
    {
        public bool IsTicking { get; } 

        /// <summary>
        /// ������ʱ��
        /// </summary>
        public void StartTimer();

        /// <summary>
        /// ֹͣ��ʱ��
        /// </summary>
        public void StopTimer();

        /// <summary>
        /// ��ͣ��ʱ��
        /// </summary>
        public void PauseTimer();

        /// <summary>
        /// ����ͣ�лָ���ʱ��
        /// </summary>
        public void RemuseTimer();

        public void Tick();
    }

    /// <summary>
    /// ��ʱ������
    /// <para>��������˵����Ӧ��ֱ�Ӵ���Timer�����ֱ��ʹ����Timer����ôһ��Ҫ����ȥ����GC�ͻص�</para>
    /// </summary>
    public class BaseTimer : IBaseTimer
    {
        #region Protected Fields

        protected bool _IsTicking = false;

        #endregion --Protected Fields

        #region Public Fields

        public bool IsTicking
        {
            get
            {
                return _IsTicking;
            }
        }

        public Action OnStart = null;
        public Action OnStop = null;
        public Action OnTick = null;
        public Action OnPause = null;
        public Action OnRemuse = null;

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods
        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods
        #endregion --Protected Methods


        public virtual void PauseTimer()
        {
            if(_IsTicking)
            {
                _IsTicking = false;
                OnPause?.Invoke();
            }
        }

        public virtual void RemuseTimer()
        {
            if (!_IsTicking)
            {
                _IsTicking = true;
                OnRemuse?.Invoke();
            }
        }

        public virtual void StartTimer()
        {
            if (!_IsTicking)
            {
                _IsTicking = true;
                OnStart?.Invoke();
            }
        }

        public virtual void StopTimer()
        {
            if (_IsTicking)
            {
                _IsTicking = false;
                OnStop?.Invoke();
            }
        }

        public virtual void Tick()
        {
            if (_IsTicking)
            {
                OnTick?.Invoke();
            }
        }
    }
}
