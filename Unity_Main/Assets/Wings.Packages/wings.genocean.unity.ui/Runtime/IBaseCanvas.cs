/*
 * FileName:    IBaseCanvas
 * Author:      Wings
 * CreateTime:  2022_01_29
 * 
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GenOcean.UI
{
    public interface IBaseCanvas
    {
        /// <summary>
        /// 打开另外一个面板
        /// </summary>
        /// <param name="PanelName"></param>
        public void OpenOtherPanel(string PanelName, object data = null);

        /// <summary>
        /// 切换到一个新面板
        /// </summary>
        public void SwitchToOtherPanel(string PanelName, object data = null);

        /// <summary>
        /// 关闭一个面板
        /// </summary>
        public void ClosePanel(string PanelName, object data = null);

        /// <summary>
        /// 切换到父面板
        /// </summary>
        public void SwitchToParentPanel();
    }

#if !USE_UI_ASYNC
    public interface IBaseCanvas<T> : IBaseCanvas
    {
        public T CurrentPanel { get; }

        /// <summary>
        /// 加载一个面板实例
        /// </summary>
        /// <param name="PanelName"></param>
        /// <returns></returns>
        public T LoadPanel(string PanelName);

        /// <summary>
        /// 切换一个面板（替换栈顶的面板）
        /// </summary>
        /// <param name="PanelName"></param>
        public T SwitchPanel(string PanelName, object data = null);

        /// <summary>
        /// 打开一个新面板
        /// </summary>
        /// <param name="PanelName"></param>
        public T OpenPanel(string PanelName, object data = null);
    }

#else
    public interface IBaseCanvas<T> : IBaseCanvas
    {
        public T CurrentPanel { get; }

        /// <summary>
        /// 加载一个面板实例
        /// </summary>
        /// <param name="PanelName"></param>
        /// <returns></returns>
        public void LoadPanel(string PanelName,Action<T> Callback = null);

        /// <summary>
        /// 切换一个面板（替换栈顶的面板）
        /// </summary>
        /// <param name="PanelName"></param>
        public void SwitchPanel(string PanelName, object data = null, Action<T> Callback = null);

        /// <summary>
        /// 打开一个新面板
        /// </summary>
        /// <param name="PanelName"></param>
        public void OpenPanel(string PanelName, object data = null, Action<T> Callback = null);
    }

#endif
}
