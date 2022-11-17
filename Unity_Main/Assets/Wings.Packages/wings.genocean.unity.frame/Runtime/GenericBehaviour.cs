/*
 * FileName:    GenericBehaviour
 * Author:      Wings
 * CreateTime:  2021_12_29
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenOcean.GameFrame
{
    /// <summary>
    /// 所有行为的基类
    /// </summary>
    public class GenericBehaviour : Pawn,IGenericBehaviour
    {
        #region Protected Fields

        protected int _BehaviourCode;
        protected BasicBehaviour _BehaviourManager;

        #endregion --Protected Fields

        #region Public Fields

        public int BehaviourCode
        {
            get
            {
                return _BehaviourCode;
            }
        }

        public bool IsSetDefaultBehaviour = false;

        public bool IsOverrideBehaviour = false;

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        public virtual void LocalFixedUpdate() { }
        public virtual void LocalLateUpdate() { }
        public virtual void OnOverride() { }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        protected virtual void SubscribeSelf()
        {

            
            if (_BehaviourManager != null)
            {
                _BehaviourManager.SubscribeBehaviour(this);
            }
#if UNITY_EDITOR || DEV_TEST
            else
            {
                Debug.Log($"{name}({GetType().Name}) can not found _BehaviourManager");
            }

#endif
        }

        /// <summary>
        /// 行为类的Input交由行为管理类进行刷新，所以这里需要取消Input的检测
        /// </summary>
        protected override void LocalCheck()
        {
            /*
            if (IsCheckInput)
            {
                CheckInput();
            }
            */

            if (IsCheckState)
            {
                CheckState();
            }

            if (IsCheckAnim)
            {
                CheckAnim();
            }
        }

        #endregion --Protected Methods

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _BehaviourCode = this.GetType().GetHashCode();
            _BehaviourManager = GetComponent<BasicBehaviour>();
        }

        protected override void Start()
        {
            base.Start();
            SubscribeSelf();

            if(IsSetDefaultBehaviour)
            {
                _BehaviourManager.RegisterDefaultBehaviour(_BehaviourCode);
            }
        }

        #endregion --Unity Methods
    }
}
