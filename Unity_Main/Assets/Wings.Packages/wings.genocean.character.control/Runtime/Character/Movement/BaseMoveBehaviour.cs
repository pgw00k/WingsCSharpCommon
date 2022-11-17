/*
 * FileName:    BaseVehicle
 * Author:      Wings
 * CreateTime:  2021_12_24
 * 
*/

using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using GenOcean.GameFrame;

namespace GenOcean.Character.Movement
{
    /// <summary>
    /// 基础移动对象
    /// <para>一个基础的Pawn角色的移动逻辑，可以复用在载具或其他物件上</para>
    /// </summary>
    public class BaseMoveBehaviour : GenericBehaviour
    {
        #region Protected Fields

        protected Vector3 _InputDirection = Vector3.zero;

        /// <summary>
        /// 移动状态
        /// <para>0 - 待机</para>
        /// <para>1 - 移动中</para>
        /// </summary>
        protected int _State = 0;

        #endregion --Protected Fields

        #region Public Fields

        public UnityEvent OnInIdle = new UnityEvent();
        public UnityEvent OnInMove = new UnityEvent();

        /// <summary>
        /// 参考系，一般来说都是摄像机
        /// </summary>
        public Transform Reference;
        public Transform Target;
        public Rigidbody Rig;
        public string InputH = "Horizontal";
        public string InputV = "Vertical";

        public float MoveSpeed = 1.0f;
        public float TurnSmoothing = 1.0f;
        public float MoveStatic = 5.0f;
        public float StepOffset = 0.3f;

        public bool IsLandUnit = true;

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        public virtual bool IsMoving()
        {
            return _InputDirection != Vector3.zero;
        }


        public override void CheckInput()
        {
            base.CheckInput();
            _InputDirection.x = Input.GetAxis(InputH);
            _InputDirection.z = Input.GetAxis(InputV);
        }


        public override void UncheckInput()
        {
            base.UncheckInput();
            _InputDirection = Vector3.zero;
        }

        public override void LocalFixedUpdate()
        {
            base.LocalFixedUpdate();

            if (IsMoving())
            {
                if (_State != 1)
                {
                    _State = 1;
                    InMoving();
                    if(OnInMove!=null)
                    {
                        OnInMove.Invoke();
                    }
                }
                else
                {
                    Rig.velocity = Moving();
                    Rotating();
                }

            }else
            {
                if(_State!=0)
                {
                    _State = 0;
                    InIdling();
                    if (OnInIdle != null)
                    {
                        OnInIdle.Invoke();
                    }
                }
                else
                {
                    Idling();
                }    
            }
        }


        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        protected virtual Vector3 Rotating()
        {
            Vector3 targetDirection = Reference.TransformDirection(_InputDirection);

            if (IsLandUnit)
            {
                // 去除Y轴上的朝向控制
                targetDirection.y = 0;
            }

            if(targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion newRotation = Quaternion.Slerp(Target.rotation, targetRotation, TurnSmoothing);
                Rig.MoveRotation(newRotation);
            }

            return targetDirection;
        }

        protected virtual Vector3 Moving()
        {
            Vector3 targetDirection = Reference.TransformDirection(_InputDirection);

            if(IsLandUnit)
            {
                // 去除Y轴上的朝向控制
                targetDirection.y = 0;
            }

            //Vector3 newPosition = Vector3.Slerp(Target.position, Target.position + targetDirection * MoveSpeed , Time.fixedDeltaTime* MoveSpeed);

            targetDirection = targetDirection * MoveSpeed * MoveStatic;
            if (IsLandUnit)
            {
                targetDirection.y = Rig.velocity.y;
            }

            //Rig.MovePosition(newPosition);

            return targetDirection;
        }

        protected virtual void Idling()
        {

        }

        protected virtual void InIdling()
        {

        }

        protected virtual void InMoving()
        {

        }

        protected virtual void CheckStep(Collision other)
        {
            if (_State == 1)
            {
#if UNITY_EDITOR
#endif
                float vdot = Vector3.Dot(Rig.velocity.normalized, Target.forward.normalized);

                if(vdot < 0.5f && other.contactCount > 0)
                {
                    Vector3[] ps = other.contacts.Select(cp => cp.point).OrderByDescending(v3 => v3.y).ToArray();
                    Vector3 p = ps[0];
                    Vector3 dir = (p - Target.position).normalized;
                    float stepHeight = p.y - Target.position.y;
                    if (Vector3.Dot(dir,Target.forward.normalized) > 0.1f && Vector3.Distance(p,Target.position) > 0.01f && stepHeight <= StepOffset && stepHeight > 0.01f)
                    {
                        Target.position = p;
                    }
                }
            }
        }

        #endregion --Protected Methods

        #region Unity Methods

        protected virtual void OnCollisionEnter(Collision other)
        {
        }

        protected virtual void OnCollisionStay(Collision other)
        {
            if (StepOffset > 0.0f)
            {
                CheckStep(other);
            }
        }

        #endregion --Unity Methods
    }
}
