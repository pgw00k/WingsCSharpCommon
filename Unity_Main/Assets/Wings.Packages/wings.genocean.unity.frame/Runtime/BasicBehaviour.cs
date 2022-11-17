/*
 * FileName:    BasicBehaviour
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
    /// 所有脚本行为的基类
    /// </summary>
    public interface IGenericBehaviour:IPawn
    {
        public int BehaviourCode { get;}
        public bool isActiveAndEnabled { get;}

        public void LocalFixedUpdate();
        public void LocalLateUpdate();
        public void OnOverride();
    }

    public interface IBasicBehaviour<T>
    {
        /// <summary>
        /// 添加一个新的行为脚本到管理列表中
        /// </summary>
        /// <param name="behaviour"></param>
        public void SubscribeBehaviour(T behaviour);

        /// <summary>
        /// 注册一个默认行为脚本
        /// <para>同时会设置改行为为当前行为</para>
        /// </summary>
        /// <param name="behaviourCode"></param>
        public void RegisterDefaultBehaviour(int behaviourCode);

        /// <summary>
        /// 激活一个行为脚本
        /// <para>只能从默认脚本进行切换</para>
        /// </summary>
        /// <param name="behaviourCode"></param>
        public void RegisterBehaviour(int behaviourCode);


        /// <summary>
        /// 反激活一个行为脚本
        /// <para>切换到默认脚本</para>
        /// </summary>
        /// <param name="behaviourCode"></param>
        public void UnregisterBehaviour(int behaviourCode);

        /// <summary>
        /// 覆写当前的行为脚本
        /// <para>用新行为脚本取代当前的行为脚本</para>
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public bool OverrideWithBehaviour(T behaviour);

        /// <summary>
        /// 取消行为脚本的覆写行为，回复到原始的脚本
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public bool RevokeOverridingBehaviour(T behaviour);

        /// <summary>
        /// 目标脚本是否处在被覆写过程中（处在被覆写过程中的脚本不会执行）
        /// <para>空参数检测当前的覆写表是否为空</para>
        /// </summary>
        /// <param name="behaviour">目标脚本</param>
        /// <returns></returns>
        public bool IsOverriding(T behaviour = default(T));

        /// <summary>
        /// 是否是当前执行的脚本行为
        /// </summary>
        /// <param name="behaviourCode"></param>
        /// <returns></returns>
        public bool IsCurrentBehaviour(int behaviourCode);

        // Check if any other behaviour is temporary locked.
        public bool GetTempLockStatus(int behaviourCodeIgnoreSelf = 0);


        // Atempt to lock on a specific behaviour.
        //  No other behaviour can overrhide during the temporary lock.
        // Use for temporary transitions like jumping, entering/exiting aiming mode, etc.
        public void LockTempBehaviour(int behaviourCode);

        // Attempt to unlock the current locked behaviour.
        // Use after a temporary transition ends.
        public void UnlockTempBehaviour(int behaviourCode);
    }

    /// <summary>
    /// 基础的行为管理基类
    /// <para>针对于Pawn类会产生的行为逻辑脚本</para>
    /// </summary>
    public class BasicBehaviour : Pawn, IBasicBehaviour<IGenericBehaviour>
    {
        #region Protected Fields

        /// <summary>
        /// 当前执行的对象行为
        /// </summary>
        protected int _CurrentBehaviour;

        /// <summary>
        /// 默认的对象行为
        /// </summary>
        protected int _DefaultBehaviour;

        /// <summary>
        /// 当前锁定的对象行为
        /// </summary>
        protected int _LockedBehaviour;

        /// <summary>
        /// 当前对象包含所有行为
        /// </summary>
        protected List<IGenericBehaviour> _Behaviours = new List<IGenericBehaviour>();
        protected List<IGenericBehaviour> _OverridingBehaviours = new List<IGenericBehaviour>();

        #endregion --Protected Fields

        #region Public Fields

        public bool IsCheckLocalFixedUpdate = true;
        public bool IsCheckLocalLateUpdate = true;

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        /// <summary>
        /// 添加一个新的行为脚本到管理列表中
        /// </summary>
        /// <param name="behaviour"></param>
        public virtual void SubscribeBehaviour(IGenericBehaviour behaviour)
        {
            _Behaviours.Add(behaviour);
        }


        /// <summary>
        /// 注册一个默认行为脚本
        /// <para>同时会设置改行为为当前行为</para>
        /// </summary>
        /// <param name="behaviourCode"></param>
        public virtual void RegisterDefaultBehaviour(int behaviourCode)
        {
            _DefaultBehaviour = behaviourCode;
            _CurrentBehaviour = behaviourCode;
        }

        /// <summary>
        /// 激活一个行为脚本
        /// <para>只能从默认脚本进行切换</para>
        /// </summary>
        /// <param name="behaviourCode"></param>
        public virtual void RegisterBehaviour(int behaviourCode)
        {
            if (_CurrentBehaviour == _DefaultBehaviour)
            {
                _CurrentBehaviour = behaviourCode;
            }
        }


        /// <summary>
        /// 反激活一个行为脚本
        /// <para>切换到默认脚本</para>
        /// </summary>
        /// <param name="behaviourCode"></param>
        public virtual void UnregisterBehaviour(int behaviourCode)
        {
            if (_CurrentBehaviour == behaviourCode)
            {
                _CurrentBehaviour = _DefaultBehaviour;
            }
        }

        // Attempt to override any active behaviour with the behaviours on queue.
        // Use to change to one or more behaviours that must overlap the active one (ex.: aim behaviour).
        /// <summary>
        /// 覆写当前的行为脚本
        /// <para>用新行为脚本取代当前的行为脚本</para>
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public virtual bool OverrideWithBehaviour(IGenericBehaviour behaviour)
        {
            // Behaviour is not on queue.
            if (!_OverridingBehaviours.Contains(behaviour))
            {
                // No behaviour is currently being overridden.
                if (_OverridingBehaviours.Count == 0)
                {
                    // Call OnOverride function of the active behaviour before overrides it.
                    foreach (IGenericBehaviour overriddenBehaviour in _Behaviours)
                    {
                        if (overriddenBehaviour.isActiveAndEnabled && _CurrentBehaviour == overriddenBehaviour.BehaviourCode)
                        {
                            overriddenBehaviour.OnOverride();
                            break;
                        }
                    }
                }
                // Add overriding behaviour to the queue.
                _OverridingBehaviours.Add(behaviour);
                return true;
            }
            return false;
        }

        // Attempt to revoke the overriding behaviour and return to the active one.
        // Called when exiting the overriding behaviour (ex.: stopped aiming).
        /// <summary>
        /// 取消行为脚本的覆写行为，回复到原始的脚本
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public virtual bool RevokeOverridingBehaviour(IGenericBehaviour behaviour)
        {
            if (_OverridingBehaviours.Contains(behaviour))
            {
                _OverridingBehaviours.Remove(behaviour);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 目标脚本是否处在被覆写过程中（处在被覆写过程中的脚本不会执行）
        /// <para>空参数检测当前的覆写表是否为空</para>
        /// </summary>
        /// <param name="behaviour">目标脚本</param>
        /// <returns></returns>
        public virtual bool IsOverriding(IGenericBehaviour behaviour = null)
        {
            if (behaviour == null)
                return _OverridingBehaviours.Count > 0;
            return _OverridingBehaviours.Contains(behaviour);
        }

        /// <summary>
        /// 是否是当前执行的脚本行为
        /// </summary>
        /// <param name="behaviourCode"></param>
        /// <returns></returns>
        public virtual bool IsCurrentBehaviour(int behaviourCode)
        {
            return this._CurrentBehaviour == behaviourCode;
        }

        // Check if any other behaviour is temporary locked.
        public virtual bool GetTempLockStatus(int behaviourCodeIgnoreSelf = 0)
        {
            return (_LockedBehaviour != 0 && _LockedBehaviour != behaviourCodeIgnoreSelf);
        }

        // Atempt to lock on a specific behaviour.
        //  No other behaviour can overrhide during the temporary lock.
        // Use for temporary transitions like jumping, entering/exiting aiming mode, etc.
        public virtual void LockTempBehaviour(int behaviourCode)
        {
            if (_LockedBehaviour == 0)
            {
                _LockedBehaviour = behaviourCode;
            }
        }

        // Attempt to unlock the current locked behaviour.
        // Use after a temporary transition ends.
        public void UnlockTempBehaviour(int behaviourCode)
        {
            if (_LockedBehaviour == behaviourCode)
            {
                _LockedBehaviour = 0;
            }
        }

        public override void CheckInput()
        {
            base.CheckInput();
            foreach (IGenericBehaviour behaviour in _Behaviours)
            {
                behaviour.CheckInput();
            }
        }

        public override void UncheckInput()
        {
            base.UncheckInput();
            foreach (IGenericBehaviour behaviour in _Behaviours)
            {
                behaviour.UncheckInput();
            }
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        /// <summary>
        /// 无主动行为时执行的内容
        /// </summary>
        protected virtual void OnNullLocalFixedUpdate()
        {
        }

        protected virtual void CheckLocalFixedUpdate()
        {
            // Call the active behaviour if no other is overriding.
            bool isAnyBehaviourActive = false;
            if (_LockedBehaviour > 0 || _OverridingBehaviours.Count == 0)
            {
                foreach (IGenericBehaviour behaviour in _Behaviours)
                {
                    if (behaviour.isActiveAndEnabled && _CurrentBehaviour == behaviour.BehaviourCode)
                    {
                        isAnyBehaviourActive = true;
                        behaviour.LocalFixedUpdate();
                    }
                }
            }
            // Call the overriding behaviours if any.
            else
            {
                foreach (IGenericBehaviour behaviour in _OverridingBehaviours)
                {
                    behaviour.LocalFixedUpdate();
                }
            }

            // 当前无主动行为，且无覆写行为时
            if (!isAnyBehaviourActive && _OverridingBehaviours.Count == 0)
            {
                OnNullLocalFixedUpdate();
            }
        }

        protected virtual void CheckLocalLateUpdate()
        {
            if (_LockedBehaviour > 0 || _OverridingBehaviours.Count == 0)
            {
                foreach (IGenericBehaviour behaviour in _Behaviours)
                {
                    if (behaviour.isActiveAndEnabled && _CurrentBehaviour == behaviour.BehaviourCode)
                    {
                        behaviour.LocalLateUpdate();
                    }
                }
            }
            // Call the overriding behaviours if any.
            else
            {
                foreach (IGenericBehaviour behaviour in _OverridingBehaviours)
                {
                    behaviour.LocalLateUpdate();
                }
            }
        }


        #endregion --Protected Methods

        #region Unity Methods

        protected virtual void FixedUpdate()
        {
            if(IsCheckLocalFixedUpdate)
            {
                CheckLocalFixedUpdate();
            }
        }

        protected virtual void LateUpdate()
        {
            if (IsCheckLocalLateUpdate)
            {
                CheckLocalLateUpdate();
            }
        }

        #endregion --Unity Methods
    }
}
