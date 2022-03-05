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
    /// ���Զ����������Timer
    /// </summary>
    public class TimerAutoGC : BaseTimer
    {
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public float Time;

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public float Durtion;

        public float DeltaTime;

        /// <summary>
        /// ѭ������
        /// <para>����Ϊ-1Ϊ����ѭ������ʱ��Timer����GC</para>
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
    /// �δ������
    /// <para>�δ���������Կ����óض���ȥ�ع� -- 2022_01_11</para>
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
        /// ע��һ����ʱ��
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
        /// ����һ����ʱ��
        /// <para>�κ�ʱ��ʹ��Start�������ļ�ʱ����һ�������� OnStart �����Ҽ�ʱ��ص�0��ʼ</para>
        /// </summary>
        /// <param name="tId"></param>
        public static void StartTimer(int tId)
        {
            Instance.InstanceStartTimer(tId);
        }

        /// <summary>
        /// ֹͣһ����ʱ��
        /// </summary>
        /// <param name="tId"></param>
        public static void StopTimer(int tId)
        {
            Instance.InstanceStopTimer(tId);
        }

        /// <summary>
        /// ֹͣһ����ʱ��
        /// </summary>
        /// <param name="tId"></param>
        public static void PauseTimer(int tId)
        {
            Instance.InstancePauseTimer(tId);
        }

        /// <summary>
        /// ֹͣһ����ʱ��
        /// </summary>
        /// <param name="tId"></param>
        public static void RemuseTimer(int tId)
        {
            Instance.InstanceRemuseTimer(tId);
        }


        /// <summary>
        /// ���һ��
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
        /// ע��һ����ʱ��
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
        /// ���һ��
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
                // �������
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

                        // ��ʱ�����
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
