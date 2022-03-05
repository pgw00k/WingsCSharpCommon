/*
 * FileName:    EventManager
 * Author:      Wings
 * CreateTime:  2021_12_03
 * 
*/

using System;
using System.Collections;
using System.Collections.Generic;

using  EventCallBack = System.Action<object>;

namespace GenOcean.Common
{

    public interface IRegsiterEvents
    {
        public bool IsRegister { get; set; }
        public Dictionary<int,int> EventIDs { get; set; }
        public void RegsiterEvents();
        public void UnregsiterEvents();
        public void RegsiterEvent(int eid, EventCallBack cb, bool isSwallow = false, int invokeCount = 1);
    }

    public class BaseEvent
    {
        public int UID = 0;
        public int EventID = 0;
        public int InvokeCount = 1;
        public bool IsSwallow = false;
        public EventCallBack Callback = null;
    }

    public class EventListener
    {
        public int EventID = 0;
        public List<BaseEvent> Events = new List<BaseEvent>();
    }

    public class EventManager : SingletonManagerBase<EventManager>
    {
        #region Protected Fields

        protected Dictionary<int, EventListener> _EventListeners = new Dictionary<int, EventListener>();

        #endregion --Protected Fields

        #region Public Fields

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        /// <summary>
        /// 注册一个监听事件回调
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="cb"></param>
        /// <param name="isSwallow"></param>
        /// <param name="invokeCount">默认只触发一次，设置为 -1 时会无限触发</param>
        public static int RegisterEventCallback(int eid, EventCallBack cb, bool isSwallow = false, int invokeCount = 1)
        {
            return Instance.InstanceRegisterEventCallback(eid, cb, isSwallow, invokeCount);
        }

        /// <summary>
        /// 分发一个指定事件
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="edata"></param>
        public static void DispatchEvent(int eid, object edata = null)
        {
            Instance.InstanceDispatchEvent(eid, edata);
        }

        /// <summary>
        /// 清除部分有问题的回调
        /// </summary>
        public static void ClearNullEvent()
        {
            Instance.InstanceClearNullEvent();
        }

        /// <summary>
        /// 取消一个事件监听
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="eid"></param>
        public static void ReleaseEventCallback(int uid,int eid)
        {
            Instance.InstanceReleaseEventCallback(uid,eid);
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        protected virtual int InstanceRegisterEventCallback(int eid, EventCallBack cb, bool isSwallow = false, int invokeCount = 1)
        {
            BaseEvent newEvent = new BaseEvent();
            newEvent.EventID = eid;
            newEvent.Callback = cb;
            newEvent.IsSwallow = false;
            newEvent.InvokeCount = invokeCount;
            newEvent.UID = newEvent.GetHashCode();

            lock (_LockObject)
            {
                EventListener el = null;
                if (!_EventListeners.TryGetValue(newEvent.EventID, out el))
                {
                    el = new EventListener();
                    el.EventID = newEvent.EventID;
                    _EventListeners.Add(newEvent.EventID, el);
                }
                el.Events.Add(newEvent);

                return newEvent.UID;
            }
        }

        protected virtual bool InstanceReleaseEventCallback(int uid,int eid)
        {
            lock (_LockObject)
            {
                EventListener el = null;
                if (_EventListeners.TryGetValue(eid, out el))
                {
                    try
                    {
                        BaseEvent bEvent = el.Events.Find(newEvent => newEvent.UID == uid);
                        el.Events.Remove(bEvent);
                        
                        return true;
                    }
                    catch (Exception err)
                    {
                        LoggerManager.LogInfo($"Can not get {eid} with UID={uid}:{err.Message}");
                        return false;
                    }
                }

                return false;
            }
        }

        protected virtual void InstanceDispatchEvent(int eid, object edata)
        {
            lock (_LockObject)
            {
                EventListener el = null;
                if (_EventListeners.TryGetValue(eid, out el))
                {
                    int index = el.Events.Count;
                    index--;
                    while (index >= 0)
                    {
                        BaseEvent e = el.Events[index];
                        try
                        {
                            if (e.InvokeCount > 0 || e.InvokeCount < 0)
                            {
                                if(e.InvokeCount > 0)
                                {
                                    e.InvokeCount--;
                                }

                                if (e.Callback != null && e.Callback.Target != null)
                                {
                                    e.Callback(edata);
                                }

                                if (e.InvokeCount == 0)
                                {
                                    el.Events.RemoveAt(index);
                                }
                                index--;
                                if(e.IsSwallow)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            el.Events.RemoveAt(index);
                            LoggerManager.LogInfo($"{GetType().Name}.InstanceDispatchEvent:{err.Message}");
                        }
                    }
                }
            }
        }



        protected virtual void InstanceClearNullEvent()
        {
            lock (_LockObject)
            {
                foreach(var el in _EventListeners.Values)
                {
                    int index = el.Events.Count;
                    index--;
                    while (index >= 0)
                    {     
                        try
                        {
                            BaseEvent e = el.Events[index];
                            if (e == null || e.Callback.Target == null)
                            {
                                el.Events.RemoveAt(index);
                            }
                        }
                        catch (Exception err)
                        {
                            el.Events.RemoveAt(index);
                            LoggerManager.LogInfo($"{GetType().Name}.InstanceClearNullEvent:{err.Message}");
                        }

                        index--;
                    }
                }

            }
        }


#endregion --Protected Methods
    }
}
