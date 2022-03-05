using System.Collections;
using System.Collections.Generic;
using System;

namespace GenOcean.Common
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public class LoggerManager:SingletonManagerBase<LoggerManager>
    {

        protected Action<string> _LogActionCallback = null;

        /// <summary>
        /// 处理最终日志打印的逻辑
        /// </summary>
        /// <param name="info"></param>
        protected void LogInfoInstance(string info)
        {
            try
            {
                if(_LogActionCallback!=null && _LogActionCallback.Target!=null)
                {
                    _LogActionCallback(info);
                }
            }catch(Exception err)
            {
                Console.WriteLine($"{GetType().Name}.LogInfoInstance:{err.Message}");
                _LogActionCallback = null;
            }
        }

        /// <summary>
        /// 注册一个日志回调
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="isReplaceAll"></param>
        public static void RegisterCallback(Action<string> cb,bool isReplaceAll = false)
        {
            if(isReplaceAll)
            {
                Instance._LogActionCallback = cb;
            }
            else
            {
                Instance._LogActionCallback += cb;
            }       
        }

        /// <summary>
        /// 输出普通日志
        /// </summary>
        /// <param name="info"></param>
        public static void LogInfo(string info)
        {
            Instance.LogInfoInstance(info);
        }

        /// <summary>
        /// 输出普通日志
        /// </summary>
        /// <param name="info"></param>
        public static void LogError(string info)
        {
            Instance.LogInfoInstance(info);
        }
    }
}
