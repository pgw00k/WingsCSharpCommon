/*
 * FileName:    BaseCanvas_Methods_Async
 * Author:      Wings
 * CreateTime:  2022_01_29
 * 
*/

#if USE_UI_ASYNC

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GenOcean.UI
{
    public partial class BaseCanvas<T>
    {
        #region Protected Fields

        

        #endregion --Protected Fields

        #region Public Fields
        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        /// <summary>
        /// 加载一个面板实例
        /// </summary>
        /// <param name="PanelName"></param>
        /// <returns></returns>
        public virtual void LoadPanel(string PanelName, Action<T> Callback = null)
        {
        }

        /// <summary>
        /// 切换一个面板（替换栈顶的面板）
        /// </summary>
        /// <param name="PanelName"></param>
        public virtual void SwitchPanel(string PanelName, object data = null, Action<T> Callback = null)
        {
            T oldPanel = null;
            if (_CurrentPanels.Count > 0)
            {
                oldPanel = _CurrentPanels.Pop();
                oldPanel.Close();
#if UNITY_EDITOR || DEV_TEST
                Debug.Log($"Try to close {oldPanel.name}");
#endif
            }

            Action<T> cbOpen = (newPanel) =>
            {
                if (oldPanel != null)
                {
                    newPanel.ParentPanelName = oldPanel.name;
                }

                if(Callback!=null)
                {
                    Callback(newPanel);
                }
            };

            OpenPanel(PanelName, data, cbOpen);
        }

        /// <summary>
        /// 打开一个新面板
        /// </summary>
        /// <param name="PanelName"></param>
        public virtual void OpenPanel(string PanelName, object data = null, Action<T> Callback = null)
        {
            if (!_Panels.ContainsKey(PanelName))
            {
                Action<T> cbLoad = (newPanel) =>
                {
                    newPanel.Init(data);
                    _Panels.Add(PanelName, newPanel);
                    T panel = OpenExistPanel(PanelName);
                    if(Callback!=null)
                    {
                        Callback(panel);
                    }
                };

                LoadPanel(PanelName, cbLoad);
            }
            else
            {
                OpenExistPanel(PanelName);
            }
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods



        #endregion --Protected Methods

        #region Unity Methods



        #endregion --Unity Methods
    }
}

#endif
