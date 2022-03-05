using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GenOcean.UI
{

    public interface IBasePanel
    {

        /// <summary>
        /// 父级面板的名称（ID）
        /// </summary>
        public string ParentPanelName { get; set; }

        /// <summary>
        /// 面板类型
        /// </summary>
        public int PanelType { get; set; }

        /// <summary>
        /// 关闭面板
        /// </summary>
        public void Close();

        /// <summary>
        /// 打开面板
        /// </summary>
        public void Open(bool isRefresh = true, object data = null);

        /// <summary>
        /// 面板初始化
        /// </summary>
        public void Init(object data = null);

        /// <summary>
        /// 刷新面板
        /// </summary>
        public void Refresh(object data = null);

        /// <summary>
        /// 设置关联画布和层级
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ca"></param>
        /// <param name="lyr"></param>
        public void SetCanvas(IBaseCanvas ca, RectTransform lyr);
    }

    public class BasePanel : MonoBehaviour, IBasePanel
    {
        protected IBaseCanvas _Canvas;
        protected RectTransform _LayerRect;

        public int PanelTypePreset = 0;

        public Action<BasePanel> OnClose = null;
        public Action<BasePanel,object> OnOpen = null;
        public Action<BasePanel,object> OnInit = null;
        public Action<BasePanel,object> OnRefresh = null;

        public UnityEvent OnOpenPanel;
        public UnityEvent OnClosePanel;

        public string _ParentPanelName = null;

        public int PanelType {
            get
            {
                return PanelTypePreset;
            }
            set
            {
                PanelTypePreset = value;
            }
        }

        public string ParentPanelName
        {
            get
            {
                return _ParentPanelName;
            }
            set
            {
                _ParentPanelName = value;
            }
        }

        //[SerializeField]

        /// <summary>
        /// 所处画布
        /// </summary>
        public IBaseCanvas Canvas
        {
            get
            {
                return _Canvas;
            }
        }

        /// <summary>
        /// 所处层级对象
        /// </summary>
        public RectTransform LayerRect
        {
            get
            {
                return _LayerRect;
            }
        }

        public virtual void SetCanvas(IBaseCanvas ca, RectTransform lyr)
        {
            _Canvas = ca;
            _LayerRect = lyr;

#if UNITY_EDITOR
            if(ca == default(IBaseCanvas))
            {
                Debug.Log($"{name}({GetType().Name}) Set Canvas fail.");
            }
#endif
        }

        /// <summary>
        /// 设置UI事件
        /// </summary>
        public virtual void SetUIEvent()
        {

        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        public virtual void Close()
        {
            if(OnClose!=null)
            {
                OnClose.Invoke(this);
            }

            gameObject.SetActive(false);
            _Canvas.ClosePanel(name, null);

            if (OnClosePanel != null)
            {
                OnClosePanel.Invoke();
            }
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        public virtual void Open(bool isRefresh = true,object data = null)
        {
            if (OnOpen != null)
            {
                OnOpen.Invoke(this,data);
            }

            if(isRefresh)
            {
                Refresh(data);
            }

            gameObject.SetActive(true);

            if(OnOpenPanel != null)
            {
                //OnOpenPanel.Invoke();
            }
        }

        /// <summary>
        /// 面板初始化
        /// </summary>
        public virtual void Init(object data = null)
        {
            if (OnInit != null)
            {
                OnInit.Invoke(this,data);
            }
        }

        /// <summary>
        /// 刷新面板
        /// </summary>
        public virtual void Refresh(object data = null)
        {
            if (OnRefresh != null)
            {
                OnRefresh.Invoke(this,data);
            }
        }

        /// <summary>
        /// 打开一个新面板（叠加不替换）
        /// </summary>
        /// <param name="PanelName"></param>
        public virtual void OpenOtherPanel(string PanelName,object data = null)
        {
            if(PanelName != name)
            {
                _Canvas.OpenOtherPanel(PanelName,data);
            }else
            {
                Debug.LogWarningFormat("{0}.OpenOtherPanel try to open {1} is same as self.", name, PanelName);
            }
        }

        /// <summary>
        /// 打开一个新面板（叠加不替换）
        /// </summary>
        /// <param name="PanelName"></param>
        public virtual void OpenOtherPanel(string PanelName)
        {
            OpenOtherPanel(PanelName, null);
        }

        /// <summary>
        /// 切换到新面板（替换不保留）
        /// </summary>
        /// <param name="PanelName"></param>
        public virtual void SwitchToOtherPanel(string PanelName)
        {
            if (PanelName != name)
            {
                _Canvas.SwitchToOtherPanel(PanelName);
            }
        }

        public virtual void SwitchToParentPanel()
        {
            _Canvas.SwitchToParentPanel();
        }
    }
}
