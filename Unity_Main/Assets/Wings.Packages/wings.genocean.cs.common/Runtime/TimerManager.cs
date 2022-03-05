/*
 * FileName:    TimerManager
 * Author:      Wings
 * CreateTime:  2022_01_11
 * 
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace GenOcean.Common
{
    /// <summary>
    /// 会自动清理自身的Timer
    /// </summary>
    public class TimerAutoGC : BaseTimer
    {
        /// <summary>
        /// 运行时间
        /// </summary>
        public float Time;

        /// <summary>
        /// 持续时间
        /// </summary>
        public float Durtion;

        public float DeltaTime;

        /// <summary>
        /// 循环次数
        /// <para>设置为-1为无限循环，此时的Timer不会GC</para>
        /// </summary>
        public int LoopCount;

        public int ID;

        public override void StartTimer()
        {
            base.StartTimer();
            Time = 0.0f;
        }

        public override void Tick()
        {
            base.Tick();
            Time += DeltaTime;
        }
    }



    /// <summary>
    /// 滴答管理器
    /// <para>滴答管理器可以考虑用池对象去重构 -- 2022_01_11</para>
    /// </summary>
    public class TimerManager : SingletonManagerBase<TimerManager>
    {
        #region Protected Fields

        protected List<TimerAutoGC> _Timers = new List<TimerAutoGC>();
        protected List<TimerAutoGC> _TimerIdle = new List<TimerAutoGC>();

        #endregion --Protected Fields

        #region Public Fields


        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        /// <summary>
        /// 注册一个计时器
        /// </summary>
        /// <param name="durtion"></param>
        /// <param name="loopCount"></param>
        /// <param name="autoStart"></param>
        /// <param name="onstart"></param>
        /// <param name="onstop"></param>
        /// <param name="ontick"></param>
        /// <param name="onpause"></param>
        /// <param name="onremuse"></param>
        public static int RegisterTimer(float durtion = 1.0f, int loopCount = 1, bool autoStart = true,
            Action onstart = null,
            Action onstop = null,
            Action ontick = null,
            Action onpause = null,
            Action onremuse = null
            )
        {
            return Instance.InstanceRegisterTimer(durtion, loopCount, autoStart, onstart, onstop, ontick, onpause, onremuse);
        }

        /// <summary>
        /// 启动一个计时器
        /// <para>任何时候，使用Start来启动的计时器，一定会运行 OnStart ，而且计时会回到0开始</para>
        /// </summary>
        /// <param name="tId"></param>
        public static void StartTimer(int tId)
        {
            Instance.InstanceStartTimer(tId);
        }

        /// <summary>
        /// 停止一个计时器
        /// </summary>
        /// <param name="tId"></param>
        public static void StopTimer(int tId)
        {
            Instance.InstanceStopTimer(tId);
        }

        /// <summary>
        /// 停止一个计时器
        /// </summary>
        /// <param name="tId"></param>
        public static void PauseTimer(int tId)
        {
            Instance.InstancePauseTimer(tId);
        }

        /// <summary>
        /// 停止一个计时器
        /// </summary>
        /// <param name="tId"></param>
        public static void RemuseTimer(int tId)
        {
            Instance.InstanceRemuseTimer(tId);
        }


        /// <summary>
        /// 嘀嗒一次
        /// </summary>
        /// <param name="delta"></param>
        public static void Tick(float delta)
        {
            Instance.InstanceTick(delta);
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        protected virtual TimerAutoGC TryGetTimer(int tid, out bool isOnIdle)
        {
            isOnIdle = false;
            TimerAutoGC bTimer = null;
            try
            {
                bTimer = _Timers.Find(bt => bt.ID == tid); 
                if(bTimer == null)
                {
                    bTimer = _TimerIdle.Find(bt => bt.ID == tid);
                    isOnIdle = (bTimer != null);
                }
            }
            catch
#if UNITY_EDITOR || DEV_TEST
            (Exception err)
#endif
            {
#if UNITY_EDITOR || DEV_TEST
                LoggerManager.LogError($"TryGetTimer:{err.Message}");
#endif
            }
            return bTimer;
        }

        /// <summary>
        /// 注册一个计时器
        /// </summary>
        /// <param name="durtion"></param>
        /// <param name="loopCount"></param>
        /// <param name="autoStart"></param>
        /// <param name="onstart"></param>
        /// <param name="onstop"></param>
        /// <param name="ontick"></param>
        /// <param name="onpause"></param>
        /// <param name="onremuse"></param>
        protected virtual int InstanceRegisterTimer(float durtion = 1.0f, int loopCount = 1, bool autoStart = true,
            Action onstart = null,
            Action onstop = null,
            Action ontick = null,
            Action onpause = null,
            Action onremuse = null
            )
        {
            TimerAutoGC bTimer = new TimerAutoGC();
            bTimer.Durtion = durtion;
            bTimer.LoopCount = loopCount;
            bTimer.OnStart += onstart;
            bTimer.OnStop += onstop;
            bTimer.OnTick += ontick;
            bTimer.OnPause += onpause;
            bTimer.OnRemuse += onremuse;
            bTimer.ID = bTimer.GetHashCode();

            if (autoStart)
            {
                bTimer.StartTimer();
                _Timers.Add(bTimer);
            }
            else
            {
                _TimerIdle.Add(bTimer);
            }

            return bTimer.ID;
        }

        /// <summary>
        /// 嘀嗒一次
        /// </summary>
        /// <param name="delta"></param>
        protected virtual void InstanceTick(float delta = 1.0f)
        {
            if (_Timers == null || _Timers.Count <= 0)
            {
                return;
            }
            lock (_LockObject)
            {
                // 逆向遍历
                int count = _Timers.Count;
                int index = count - 1;
                while (index >= 0)
                {
                    TimerAutoGC bTimer = _Timers[index];

                    if (!bTimer.IsTicking)
                    {
                        goto End;
                    }

                    bTimer.DeltaTime = delta;
                    bTimer.Tick();
                    if (bTimer.Time > bTimer.Durtion)
                    {
                        bTimer.StopTimer();

                        // 计时器完成
                        if (bTimer.LoopCount > 0)
                        {
                            bTimer.LoopCount--;
                            if (bTimer.LoopCount == 0)
                            {
                                _TimerIdle.Add(bTimer);
                                _Timers.RemoveAt(index);
                            }
                        }
                    }
                End:
                    index--;
                }
            }
        }

        protected virtual void InstanceStartTimer(int tid)
        {
            bool isInIdle = false;
            TimerAutoGC bTimer = TryGetTimer(tid, out isInIdle);
            if (bTimer != null)
            {
                bTimer.StartTimer();
                if (isInIdle)
                {
                    _TimerIdle.Remove(bTimer);
                    _Timers.Add(bTimer);
                }
            }
#if UNITY_EDITOR || DEV_TEST
            else
            {
                LoggerManager.LogInfo($"{tid} can not found");
            }
#endif
        }

        protected virtual void InstanceStopTimer(int tid)
        {
            bool isInIdle = false;
            TimerAutoGC bTimer = TryGetTimer(tid, out isInIdle);
            if (bTimer != null)
            {
                bTimer.StopTimer();
            }
        }

        protected virtual void InstancePauseTimer(int tid)
        {
            bool isInIdle = false;
            TimerAutoGC bTimer = TryGetTimer(tid, out isInIdle);
            if (bTimer != null)
            {
                bTimer.PauseTimer();
            }
        }

        protected virtual void InstanceRemuseTimer(int tid)
        {
            bool isInIdle = false;
            TimerAutoGC bTimer = TryGetTimer(tid, out isInIdle);
            if (bTimer != null)
            {
                bTimer.RemuseTimer();
            }
        }

#endregion --Protected Methods
    }
}
