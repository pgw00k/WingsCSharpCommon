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
    /// ���нű���Ϊ�Ļ���
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
        /// ���һ���µ���Ϊ�ű��������б���
        /// </summary>
        /// <param name="behaviour"></param>
        public void SubscribeBehaviour(T behaviour);

        /// <summary>
        /// ע��һ��Ĭ����Ϊ�ű�
        /// <para>ͬʱ�����ø���ΪΪ��ǰ��Ϊ</para>
        /// </summary>
        /// <param name="behaviourCode"></param>
        public void RegisterDefaultBehaviour(int behaviourCode);

        /// <summary>
        /// ����һ����Ϊ�ű�
        /// <para>ֻ�ܴ�Ĭ�Ͻű������л�</para>
        /// </summary>
        /// <param name="behaviourCode"></param>
        public void RegisterBehaviour(int behaviourCode);


        /// <summary>
        /// ������һ����Ϊ�ű�
        /// <para>�л���Ĭ�Ͻű�</para>
        /// </summary>
        /// <param name="behaviourCode"></param>
        public void UnregisterBehaviour(int behaviourCode);

        /// <summary>
        /// ��д��ǰ����Ϊ�ű�
        /// <para>������Ϊ�ű�ȡ����ǰ����Ϊ�ű�</para>
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public bool OverrideWithBehaviour(T behaviour);

        /// <summary>
        /// ȡ����Ϊ�ű��ĸ�д��Ϊ���ظ���ԭʼ�Ľű�
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public bool RevokeOverridingBehaviour(T behaviour);

        /// <summary>
        /// Ŀ��ű��Ƿ��ڱ���д�����У����ڱ���д�����еĽű�����ִ�У�
        /// <para>�ղ�����⵱ǰ�ĸ�д���Ƿ�Ϊ��</para>
        /// </summary>
        /// <param name="behaviour">Ŀ��ű�</param>
        /// <returns></returns>
        public bool IsOverriding(T behaviour = default(T));

        /// <summary>
        /// �Ƿ��ǵ�ǰִ�еĽű���Ϊ
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
    /// ��������Ϊ�������
    /// <para>�����Pawn����������Ϊ�߼��ű�</para>
    /// </summary>
    public class BasicBehaviour : Pawn, IBasicBehaviour<IGenericBehaviour>
    {
        #region Protected Fields

        /// <summary>
        /// ��ǰִ�еĶ�����Ϊ
        /// </summary>
        protected int _CurrentBehaviour;

        /// <summary>
        /// Ĭ�ϵĶ�����Ϊ
        /// </summary>
        protected int _DefaultBehaviour;

        /// <summary>
        /// ��ǰ�����Ķ�����Ϊ
        /// </summary>
        protected int _LockedBehaviour;

        /// <summary>
        /// ��ǰ�������������Ϊ
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
        /// ���һ���µ���Ϊ�ű��������б���
        /// </summary>
        /// <param name="behaviour"></param>
        public virtual void SubscribeBehaviour(IGenericBehaviour behaviour)
        {
            _Behaviours.Add(behaviour);
        }


        /// <summary>
        /// ע��һ��Ĭ����Ϊ�ű�
        /// <para>ͬʱ�����ø���ΪΪ��ǰ��Ϊ</para>
        /// </summary>
        /// <param name="behaviourCode"></param>
        public virtual void RegisterDefaultBehaviour(int behaviourCode)
        {
            _DefaultBehaviour = behaviourCode;
            _CurrentBehaviour = behaviourCode;
        }

        /// <summary>
        /// ����һ����Ϊ�ű�
        /// <para>ֻ�ܴ�Ĭ�Ͻű������л�</para>
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
        /// ������һ����Ϊ�ű�
        /// <para>�л���Ĭ�Ͻű�</para>
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
        /// ��д��ǰ����Ϊ�ű�
        /// <para>������Ϊ�ű�ȡ����ǰ����Ϊ�ű�</para>
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
        /// ȡ����Ϊ�ű��ĸ�д��Ϊ���ظ���ԭʼ�Ľű�
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
        /// Ŀ��ű��Ƿ��ڱ���д�����У����ڱ���д�����еĽű�����ִ�У�
        /// <para>�ղ�����⵱ǰ�ĸ�д���Ƿ�Ϊ��</para>
        /// </summary>
        /// <param name="behaviour">Ŀ��ű�</param>
        /// <returns></returns>
        public virtual bool IsOverriding(IGenericBehaviour behaviour = null)
        {
            if (behaviour == null)
                return _OverridingBehaviours.Count > 0;
            return _OverridingBehaviours.Contains(behaviour);
        }

        /// <summary>
        /// �Ƿ��ǵ�ǰִ�еĽű���Ϊ
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
        /// ��������Ϊʱִ�е�����
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

            // ��ǰ��������Ϊ�����޸�д��Ϊʱ
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
