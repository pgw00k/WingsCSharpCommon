using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GenOcean.UI
{
    /// <summary>
    /// Layers:
    /// <para>0 - PanelLayer</para>
    /// <para>1 - TipLayer</para>
    /// <para>2 - BackGroundLayer</para>
    /// <para>3 - ExternalLayer</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [RequireComponent(typeof(Canvas))]
    public partial class BaseCanvas<T> : MonoBehaviour, IBaseCanvas<T>
        where T : MonoBehaviour, IBasePanel
    {
        /// <summary>
        /// 画布的根节点
        /// </summary>
        public RectTransform CanvasRoot;


        /// <summary>
        /// Layers:
        /// <para>0 - PanelLayer</para>
        /// <para>1 - TipLayer</para>
        /// <para>2 - BackGroundLayer</para>
        /// <para>3 - ExternalLayer</para>
        /// </summary>
        public RectTransform[] Layers;

        #region 自定义层级

        /// <summary>
        /// 背景层
        /// </summary>
        public RectTransform BackgroundLayer
        {
            get
            {
                return Layers[2];
            }
        }

        /// <summary>
        /// 菜单面板层（默认层级）
        /// </summary>
        public RectTransform PanelLayer
        {
            get
            {
                return Layers[0];
            }
        }

        /// <summary>
        /// 额外层（可用于部分全局按钮所在的面板，比如随时可以打开的设置）
        /// </summary>
        public RectTransform ExternalLayer
        {
            get
            {
                return Layers[3];
            }
        }

        /// <summary>
        /// 提示面板层（最顶层）
        /// </summary>
        public RectTransform TipLayer
        {
            get
            {
                return Layers[1];
            }
        }

        #endregion

        /// <summary>
        /// 储存已经加载好的面板
        /// </summary>
        protected Dictionary<string, T> _Panels = new Dictionary<string, T>();

        /// <summary>
        /// 当前菜单面板层的所有面板
        /// </summary>
        protected Stack<T> _CurrentPanels = new Stack<T>();


        public T CurrentPanel
        {
            get
            {
                if(_CurrentPanels.Count>0)
                {
                    return _CurrentPanels.Peek();
                }
//#if DEBUG_LOG && DEBUG_ERROR
#if UNITY_EDITOR
                else
                {
                    //Logger.Log($"{name}({GetType()}) CurrentPanel is null.（_CurrentPanels(Stack) is empty.）");
                }
#endif
                return default(T);
            }
        }

        public virtual void SetPanelInit(T panel, string PanelName)
        {
            if (!panel)
            {
#if UNITY_EDITOR
                Debug.Log($"{name}({GetType().Name}) Load {PanelName} fail.");
#endif
                return;
            }

            // 在没有正式Open之前，将其设置为不激活状态
            panel.gameObject.SetActive(false);
            RectTransform rtParentLayer = Layers[panel.PanelType];

            panel.SetCanvas(this, rtParentLayer);
            panel.name = PanelName;
            panel.transform.SetParent(rtParentLayer);
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localRotation = Quaternion.identity;
            panel.transform.localScale = Vector3.one;


            RectTransform rt = panel.transform as RectTransform;
            if (rt)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
        }

        public virtual void ClosePanel(string PanelName, object data = null)
        {
            if (_CurrentPanels.Count > 0)
            {
                T oldPanel = _CurrentPanels.Peek();
#if UNITY_EDITOR || DEV_TEST
                Debug.Log($"Try to close {oldPanel.name}");
#endif
                if (oldPanel.name == PanelName)
                {
                    oldPanel = _CurrentPanels.Pop();
                }
#if UNITY_EDITOR || DEV_TEST
                else
                {
                    Debug.Log($"Try to close {PanelName} but get {oldPanel.name}");
                }          
#endif
            }
        }

        public virtual void OpenOtherPanel(string PanelName, bool isRefresh = true,object data = null)
        {
            OpenPanel(PanelName, isRefresh, data);
        }

        public virtual void SwitchToOtherPanel(string PanelName, bool isRefresh = true, object data = null)
        {
            SwitchPanel(PanelName,isRefresh, data);
        }

        /// <summary>
        /// 关闭当前面板，打开上一级面板
        /// </summary>
        /// <returns></returns>
        public virtual void SwitchToParentPanel()
        {
            T oldPanel = null;
            T newPanel = null;
            if (_CurrentPanels.Count > 0)
            {
                oldPanel = _CurrentPanels.Pop();

                if(!string.IsNullOrEmpty(oldPanel.ParentPanelName))
                {
                    oldPanel.Close();
#if UNITY_EDITOR || DEV_TEST
                    Debug.Log($"Try to close {oldPanel.name}");
#endif

                    OpenPanel(oldPanel.ParentPanelName);
                }else
                {
                    newPanel = oldPanel;
                }
            }
        }

        protected virtual T OpenExistPanel(string PanelName, object data = null)
        {
            T panel = _Panels[PanelName];
            panel.transform.SetAsLastSibling();
            _CurrentPanels.Push(panel);
            panel.Open(true, data);

            return panel;
        }
    }
}
