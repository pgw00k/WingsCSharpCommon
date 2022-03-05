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
        /// ������һ�����
        /// </summary>
        /// <param name="PanelName"></param>
        public void OpenOtherPanel(string PanelName, object data = null);

        /// <summary>
        /// �л���һ�������
        /// </summary>
        public void SwitchToOtherPanel(string PanelName, object data = null);

        /// <summary>
        /// �ر�һ�����
        /// </summary>
        public void ClosePanel(string PanelName, object data = null);

        /// <summary>
        /// �л��������
        /// </summary>
        public void SwitchToParentPanel();
    }

#if !USE_UI_ASYNC
    public interface IBaseCanvas<T> : IBaseCanvas
    {
        public T CurrentPanel { get; }

        /// <summary>
        /// ����һ�����ʵ��
        /// </summary>
        /// <param name="PanelName"></param>
        /// <returns></returns>
        public T LoadPanel(string PanelName);

        /// <summary>
        /// �л�һ����壨�滻ջ������壩
        /// </summary>
        /// <param name="PanelName"></param>
        public T SwitchPanel(string PanelName, object data = null);

        /// <summary>
        /// ��һ�������
        /// </summary>
        /// <param name="PanelName"></param>
        public T OpenPanel(string PanelName, object data = null);
    }

#else
    public interface IBaseCanvas<T> : IBaseCanvas
    {
        public T CurrentPanel { get; }

        /// <summary>
        /// ����һ�����ʵ��
        /// </summary>
        /// <param name="PanelName"></param>
        /// <returns></returns>
        public void LoadPanel(string PanelName,Action<T> Callback = null);

        /// <summary>
        /// �л�һ����壨�滻ջ������壩
        /// </summary>
        /// <param name="PanelName"></param>
        public void SwitchPanel(string PanelName, object data = null, Action<T> Callback = null);

        /// <summary>
        /// ��һ�������
        /// </summary>
        /// <param name="PanelName"></param>
        public void OpenPanel(string PanelName, object data = null, Action<T> Callback = null);
    }

#endif
}
