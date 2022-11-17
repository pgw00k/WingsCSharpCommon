/*
 * FileName:    CharacterMoveBehaviour
 * Author:      Wings
 * CreateTime:  2021_12_30
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GenOcean.Common;
using GenOcean.GameFrame;

namespace GenOcean.Character.Movement
{
    /// <summary>
    /// 需要胶囊体来进行碰撞检测
    /// </summary>
    public class CharacterMoveBehaviour : BaseMoveBehaviour
    {
        #region Protected Fields

        protected bool _IsJump;
        protected bool _IsColliding;
        protected CharacterBehaviour _CharacterBehaviour;

        protected float _CharacterMoveSpeed;

        protected Collider _Col;

        protected int _IsGroundPrev = 0;

        #endregion --Protected Fields

        #region Public Fields

        public string JumpButton = "Jump";
        public string SprintButton = "Sprint";
        public string WalkButton = "Walk";
        public float SprintSpeed = 2.0f;
        public float JogSpeed = 1.0f;
        public float WalkSpeed = 0.2f;
        public float JumpHeight = 1.5f;

        /// <summary>
        /// 跳跃式的水平惯性力
        /// </summary>
        public float JumpIntertialForce = 10f;

        public LayerMask GroundMask = 1;


        #region 影响动画效果

        public float MoveSpeedAnimation = 1.0f;

        #endregion

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        /// <summary>
        /// 是否是在地面的检测
        /// </summary>
        /// <returns></returns>
        public virtual bool IsGrounded()
        {
            Ray ray = new Ray(Target.position + Vector3.up * 2 * _Col.bounds.extents.x, Vector3.down);
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction, Color.blue, 1.0f);
#endif
            return Physics.SphereCast(ray, _Col.bounds.extents.x, _Col.bounds.extents.x + 0.2f, GroundMask);
        }

        public override void CheckInput()
        {
            base.CheckInput();

            // Get jump input.
            if (!_IsJump && Input.GetButtonDown(JumpButton) && _BehaviourManager.IsCurrentBehaviour(_BehaviourCode) && !_BehaviourManager.IsOverriding())
            {
#if UNITY_EDITOR
                Debug.Log($"{name} Jump");
#endif
                _IsJump = true;
            }

            /*
            if (Input.GetButton(SprintButton))
            {
                if(_CharacterBehaviour.Anim.Animator.applyRootMotion)
                {
                    _MoveSpeedSeek = SprintSpeed;
                }
                else
                {
                    /// 当不适用RootMotion来控制的时候，把speed值修改直接给MoveSpeed
                    
                    _MoveSpeedSeek = 2.0f;
                    MoveSpeed = SprintSpeed;
                }

            }
            else if (Input.GetButton(WalkButton))
            {
                if (_CharacterBehaviour.Anim.Animator.applyRootMotion)
                {
                    _MoveSpeedSeek = WalkSpeed;
                }
                else
                {
                    /// 当不使用RootMotion来控制的时候，把speed值修改直接给MoveSpeed
                    _MoveSpeedSeek = 0.2f;
                    MoveSpeed = WalkSpeed;
                }

            }
            else
            */
            {
                if (_CharacterBehaviour.Anim.Animator.applyRootMotion)
                {
                    //_MoveSpeedSeek = JogSpeed;
                }
                else
                {
                    MoveSpeed = JogSpeed;
                }
            }
        }

        public override void CheckAnim()
        {
            base.CheckAnim();

            // 在开启RootMotion时移动逻辑依赖于动画
            // 更新动画参数
            //if(!IsGrounded())
            //{
            //    _IsGroundPrev++;
            //    if(_IsGroundPrev >= 30)
            //    {
            //        _CharacterBehaviour.Anim.Grounded = false;
            //    }
            //}else
            //{
            //    _IsGroundPrev = 0;
            //    _CharacterBehaviour.Anim.Grounded = true;
            //}

            _CharacterBehaviour.Anim.Grounded = IsGrounded();

            _CharacterBehaviour.Anim.MoveSpeed = _InputDirection.magnitude * MoveSpeedAnimation;
            JumpManagement();
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods



        /// <summary>
        /// 人物移动默认依赖于RootMotion
        /// <para>逻辑移动取消</para>
        /// </summary>
        /// <returns></returns>
        protected override Vector3 Moving()
        {
            //Vector3 moveVector = base.Moving();

            if (_CharacterBehaviour.Anim.Animator.applyRootMotion)
            {
                return Vector3.zero;
            }
            else
            {
                return base.Moving();
            }
            
        }

        /// <summary>
        /// 跳跃逻辑刷新
        /// </summary>
        protected virtual void JumpManagement()
        {
            // Start a new jump.
            if (_IsJump && !_CharacterBehaviour.Anim.Jump && IsGrounded())
            {
                // Set jump related parameters.
                _CharacterBehaviour.LockTempBehaviour(_BehaviourCode);
                _CharacterBehaviour.Anim.Jump = true;

                // Temporarily change player friction to pass through obstacles.
                _Col.material.dynamicFriction = 0f;
                _Col.material.staticFriction = 0f;

                // Remove vertical velocity to avoid "super jumps" on slope ends.
                _CharacterBehaviour.RemoveVerticalVelocity();

                // Set jump vertical impulse velocity.
                float velocity = 2f * Mathf.Abs(Physics.gravity.y) * JumpHeight;
                velocity = Mathf.Sqrt(velocity);
                //_CharacterBehaviour.Rig.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);

                Rig.velocity += Vector3.up * velocity;

                // Is a locomotion jump?
                if (_CharacterBehaviour.Anim.MoveSpeed > 0.1)
                {

                }
            }
            // Is already jumping?
            else if (_CharacterBehaviour.Anim.Jump)
            {
                // Keep forward movement while in the air.
                if (IsGrounded() && !_IsColliding && _CharacterBehaviour.GetTempLockStatus())
                {
                    //_CharacterBehaviour.Rig.AddForce(transform.forward * JumpIntertialForce * Physics.gravity.magnitude * SprintSpeed, ForceMode.Acceleration);
                }
                // Has landed?
                if ((_CharacterBehaviour.Rig.velocity.y < 0) && IsGrounded())
                {
                    _CharacterBehaviour.Anim.Grounded = true;
                    // Change back player friction to default.
                    _Col.material.dynamicFriction = 0.6f;
                    _Col.material.staticFriction = 0.6f;
                    // Set jump related parameters.
                    _IsJump = false;
                    _CharacterBehaviour.Anim.Jump = false;
                    _CharacterBehaviour.UnlockTempBehaviour(_BehaviourCode);
                }
            }
        }

        #endregion --Protected Methods

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();

            _CharacterBehaviour = _BehaviourManager as CharacterBehaviour;
            _Col = GetComponent<Collider>();

#if UNITY_EDITOR || DEV_TEST
            if (!_CharacterBehaviour)
            {
                Debug.Log($"{name}({GetType().Name}) Get CharacterBehaviour fail.");
                Debug.Log($"{name}({GetType().Name}) _CharacterBehaviour is {_CharacterBehaviour.GetType().Name}.");
            }
#endif
        }

        // Collision detection.
        protected override void OnCollisionStay(Collision collision)
        {
            base.OnCollisionStay(collision);

            _IsColliding = true;
            // Slide on vertical obstacles
            if (_BehaviourManager.IsCurrentBehaviour(_BehaviourCode) && collision.GetContact(0).normal.y <= 0.1f)
            {
                _Col.material.dynamicFriction = 0f;
                _Col.material.staticFriction = 0f;
            }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            _IsColliding = false;
            _Col.material.dynamicFriction = 0.6f;
            _Col.material.staticFriction = 0.6f;
        }


        #endregion --Unity Methods
    }
}
