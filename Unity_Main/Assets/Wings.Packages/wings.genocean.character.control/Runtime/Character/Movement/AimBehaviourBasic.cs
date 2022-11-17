/*
 * FileName:    AimBehaviourBasic2
 * Author:      Wings
 * CreateTime:  2021_12_30
 * 
*/

/*
 *
 * 因为自定义代码不会Strip掉，所以使用预编译符来操作
 * 
 */
#define USE_CLICK_FOR_AIM

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GenOcean.Common;
using GenOcean.GameFrame;

namespace GenOcean.Character.Movement
{
    public class AimBehaviourBasic : GenericBehaviour
    {
        #region Protected Fields

        /// <summary>
        /// 是否已经处在瞄准模式下
        /// </summary>     
        [SerializeField]
        protected bool _IsAim = true;

        #endregion --Protected Fields

        #region Public Fields

        public ThirdPersonOrbitCamBasic Cam;

        public string AimButton = "Aim";

        public Vector3 AimPivotOffset = new Vector3(0.5f, 1.2f, 0f);
        public Vector3 AimCamOffset = new Vector3(0f, 0.4f, -0.7f);

        #region 动画参数

        public float AimTurnSmoothing = 0.15f;

        #endregion

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods
        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

#if USE_CLICK_FOR_AIM

        /// <summary>
        /// 单击切换
        /// </summary>
        protected virtual void ClickSwitch()
        {
            if (Input.GetButtonDown(AimButton))
            {
                bool tryAim = !_IsAim;
                if (tryAim)
                {
                    StartCoroutine(ToggleAimOn());
                }
                else
                {
                    StartCoroutine(ToggleAimOff());
                }
            }
        }

#else
        /// <summary>
        /// 通过按住来切换视角
        /// </summary>
        protected virtual void HoldingSwitch()
        {
            // Activate/deactivate aim by input.
            if (Input.GetAxisRaw(AimButton) != 0 && !_IsAim)
            {
                StartCoroutine(ToggleAimOn());
            }
            else if (_IsAim && Input.GetAxisRaw(AimButton) == 0)
            {
                StartCoroutine(ToggleAimOff());
            }
        }
#endif

        public override void CheckInput()
        {
            base.CheckInput();
#if USE_CLICK_FOR_AIM
            ClickSwitch();
#else
            HoldingSwitch();
#endif

        }


        protected virtual IEnumerator ToggleAimOn()
        {
            // Aiming is not possible.
            if (_BehaviourManager.GetTempLockStatus(_BehaviourCode) || _BehaviourManager.IsOverriding(this) || !Cam)
            {
#if UNITY_EDITOR || DEV_TEST
                Debug.Log($"{name}({GetType().Name}) Aim On fail.");
#endif
                yield return false;
            }

            // Start aiming.
            else
            {
                SingleDefaultEventManager.DispatchEvent(10001, true);
                Cam.SetTargetOffsets(AimPivotOffset, AimCamOffset);
                _IsAim = true;
                int signal = 1;
                AimCamOffset.x = Mathf.Abs(AimCamOffset.x) * signal;
                AimPivotOffset.x = Mathf.Abs(AimPivotOffset.x) * signal;

                if (IsOverrideBehaviour)
                {
                    _BehaviourManager.OverrideWithBehaviour(this);
                }
            }

        }

        protected virtual IEnumerator ToggleAimOff()
        {
            if (!Cam)
            {
#if UNITY_EDITOR || DEV_TEST
                Debug.Log($"{name}({GetType().Name}) Aim Off fail.");
#endif
                yield return false;
            }

            else
            {
                SingleDefaultEventManager.DispatchEvent(10001, false);
                _IsAim = false;
                Cam.ResetTargetOffsets();
                Cam.ResetMaxVerticalAngle();

                if (IsOverrideBehaviour)
                {
                    _BehaviourManager.RevokeOverridingBehaviour(this);
                }
                yield return true;
            }
        }

#endregion --Protected Methods

#region Unity Methods

#endregion --Unity Methods
    }
}
