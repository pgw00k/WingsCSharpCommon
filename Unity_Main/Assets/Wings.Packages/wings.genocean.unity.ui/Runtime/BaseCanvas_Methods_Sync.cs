/*
 * FileName:    BaseCanvas_Methods_Sync
 * Author:      Wings
 * CreateTime:  2022_01_29
 * 
*/

#if !USE_UI_ASYNC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenOcean.UI
{
    public partial class BaseCanvas<T>
    {
#region Protected Fields
#endregion --Protected Fields

#region Public Fields

        /// <summary>
        /// 加载一个面板实例
        /// </summary>
        /// <param name="PanelName"></param>
        /// <returns></returns>
        public virtual T LoadPanel(string PanelName)
        {
            T panel = null;
#if UNITY_EDITOR
            panel = (GameObject.Instantiate(Resources.Load("UI/" + PanelName)) as GameObject).GetComponent<T>();
#else
            panel = (GameObject.Instantiate(Resources.Load("UI/" + PanelName)) as GameObject).GetComponent<T>();
#endif
            SetPanelInit(panel, PanelName);

            return panel;
        }

        /// <summary>
        /// 切换一个面板（替换栈顶的面板）
        /// </summary>
        /// <param name="PanelName"></param>
        public virtual T SwitchPanel(string PanelName, object data = null)
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

            T newPanel = OpenPanel(PanelName, data);

            if (oldPanel != null)
            {
                newPanel.ParentPanelName = oldPanel.name;
            }

            return newPanel;
        }

        /// <summary>
        /// 打开一个新面板
        /// </summary>
        /// <param name="PanelName"></param>
        public virtual T OpenPanel(string PanelName, object data = null)
        {
            if (!_Panels.ContainsKey(PanelName))
            {
                T newPanel = LoadPanel(PanelName);
                newPanel.Init(data);
                _Panels.Add(PanelName, newPanel);
            }

            return OpenExistPanel(PanelName,data);
        }

#endregion --Public Fields

#region Private Fields
#endregion --Private Fields

#region Public Methods
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
