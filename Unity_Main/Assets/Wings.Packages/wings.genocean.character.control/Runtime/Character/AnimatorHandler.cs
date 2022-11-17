/*
 * FileName:    AnimatorHandler
 * Author:      Wings
 * CreateTime:  2022_10_27
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GenOcean.Character.Data;

namespace GenOcean.Character
{
    public interface IAnimatorParam
    {
    }

    /// <summary>
    /// 人形角色的基础动画控制工具
    /// </summary>
    public class AnimatorHandler : MonoBehaviour
    {
        #region Protected Fields

        protected Animator _Animator;

        #endregion --Protected Fields

        #region Public Fields

        public Animator Animator
        {
            get
            {
                return _Animator;
            }
        }

        public virtual float MoveSpeed
        {
            get
            {
                return _Animator.GetFloat(SingleAnimationHashManager.MoveSpeedHash);
            }
            set
            {
                _Animator.SetFloat(SingleAnimationHashManager.MoveSpeedHash, value);
            }
        }

        public virtual float Horizontal
        {
            get
            {
                return _Animator.GetFloat(SingleAnimationHashManager.HorizontalHash);
            }
            set
            {
                _Animator.SetFloat(SingleAnimationHashManager.HorizontalHash, value);
            }
        }

        public virtual float Vertical
        {
            get
            {
                return _Animator.GetFloat(SingleAnimationHashManager.VerticalHash);
            }
            set
            {
                _Animator.SetFloat(SingleAnimationHashManager.VerticalHash, value);
            }
        }

        public virtual bool Grounded
        {
            get
            {
                return _Animator.GetBool(SingleAnimationHashManager.GroundedHash);
            }
            set
            {
                _Animator.SetBool(SingleAnimationHashManager.GroundedHash, value);
            }
        }

        public virtual bool Jump
        {
            get
            {
                return _Animator.GetBool(SingleAnimationHashManager.JumpHash);
            }
            set
            {
                _Animator.SetBool(SingleAnimationHashManager.JumpHash, value);
            }
        }

        public virtual bool Fly
        {
            get
            {
                return _Animator.GetBool(SingleAnimationHashManager.FlyHash);
            }
            set
            {
                _Animator.SetBool(SingleAnimationHashManager.FlyHash, value);
            }
        }

        public virtual bool Aim
        {
            get
            {
                return _Animator.GetBool(SingleAnimationHashManager.AimHash);
            }
            set
            {
                _Animator.SetBool(SingleAnimationHashManager.AimHash, value);
            }
        }

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods
        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        protected virtual void InitHash()
        {

        }

        #endregion --Protected Methods

        #region Unity Methods

        protected virtual void Awake()
        {
            _Animator = GetComponent<Animator>();

            if(_Animator)
            {
                InitHash();
            }
#if UNITY_EDITOR || DEBUG_LOG
            else
            {
                Debug.Log($"{name} can not get Animator.");
            }
#endif
        }
        #endregion --Unity Methods
    }
}
