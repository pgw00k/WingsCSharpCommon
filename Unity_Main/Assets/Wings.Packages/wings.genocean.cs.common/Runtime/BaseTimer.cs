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
        /// 开启计时器
        /// </summary>
        public void StartTimer();

        /// <summary>
        /// 停止计时器
        /// </summary>
        public void StopTimer();

        /// <summary>
        /// 暂停计时器
        /// </summary>
        public void PauseTimer();

        /// <summary>
        /// 从暂停中恢复计时器
        /// </summary>
        public void RemuseTimer();

        public void Tick();
    }

    /// <summary>
    /// 计时器基类
    /// <para>理论上来说，不应该直接创建Timer，如果直接使用了Timer，那么一定要自行去处理GC和回调</para>
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
