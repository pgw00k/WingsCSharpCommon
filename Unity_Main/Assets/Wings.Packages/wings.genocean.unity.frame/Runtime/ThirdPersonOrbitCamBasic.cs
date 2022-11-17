/*
 * FileName:    ThirdPersonOrbitCamBasic2
 * Author:      Wings
 * CreateTime:  2021_12_29
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GenOcean.Common;

namespace GenOcean.GameFrame
{
    /// <summary>
    /// һ�������Ĺ۲����
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class ThirdPersonOrbitCamBasic : Pawn
    {
        #region Protected Fields

        /// <summary>
        /// ������Ƿ��Ǵ�����ת״̬��
        /// </summary>
        [SerializeField]
        protected bool _IsRotate = false;

        /// <summary>
        /// ������Ƿ��Ǵ�������ģʽ��
        /// <para>�������ģʽ������������������ڹ۲�����ϣ�һ����˵��ʱ���� Override �Ĳ������и���</para>
        /// </summary>
        [SerializeField]
        protected bool _IsLockCam = true;

        protected Camera _Camera;

        protected bool _IsCheckRotation= true;
        protected bool _IsCheckPosition = true;

        protected bool _IsUpdateFOV = false;

        protected float _TargetFOV;

        #region ����ʱ���м���

        protected float angleH = 0;
        protected float angleV = 0;
        protected float distanceZ = 0;
        protected float targetMaxVerticalAngle;
        protected Quaternion camYRotation;
        protected Quaternion aimRotation;
        protected Vector3 _TargetDirection;
        protected Vector3 targetPosition;
        protected Quaternion targetRotation;

        /// <summary>
        /// ������ĵ�ǰpivot
        /// </summary>
        protected Vector3 smoothPivotOffset;

        /// <summary>
        /// ������ĵ�ǰoffset
        /// </summary>
        protected Vector3 smoothCamOffset;
        protected Vector3 targetPivotOffset;
        protected Vector3 targetCamOffset;

        /// <summary>
        /// �Ƿ��������Զ���ƫ��
        /// <para>ֻ������Ϸ�����У�ͨ�������߼��������òŻ���Ч</para>
        /// <para>Ҳ����˵�������ֱ������ PivotOffset �� CamOffset ����������Ч</para>
        /// </summary>
        protected bool isCustomOffset;

        protected LayerMask _OldMask;

        #endregion

        #endregion --Protected Fields

        #region Public Fields

        /// <summary>
        /// �ο�ϵ��һ����˵�������Ĺ۲����
        /// </summary>
        public Transform Reference;

        /// <summary>
        /// ��������һ����˵������
        /// </summary>
        public Transform Target;

        /// <summary>
        /// ��ת����Offset������� Reference��
        /// </summary>
        public Vector3 PivotOffset = new Vector3(0.0f, 1.7f, 0.0f);

        /// <summary>
        /// �������Pivot��Offset
        /// </summary>
        public Vector3 CamOffset = new Vector3(0.0f, 0.0f, -3.0f);

        /// <summary>
        /// �������ҡ��ƫ�ƾ���
        /// <para>x-��Զ���룬y-�����z-��ǰƫ��</para>
        /// </summary>
        public Vector3 CamViewDistance = new Vector3(-10.0f, -0.5f, 0);

        /// <summary>
        /// Ĭ�ϵ�FOV
        /// </summary>
        public float DefaultFOV = 45;

        public string XAxis = "Mouse X";
        public string YAxis = "Mouse Y";
        public string ViewOffset = "Mouse ScrollWheel";
        public string CamRotate = "Fire1";

        /// <summary>
        /// ������ı��ϼ���
        /// </summary>
        public LayerMask CollisionLayer = 1;

        /// <summary>
        /// Ŀ��㼶
        /// </summary>
        public LayerMask TargetLayer = 1;

        /// <summary>
        /// �Ƿ������ϼ��
        /// </summary>
        public bool IsCheckCollision = true;

#if UNITY_EDITOR
        public bool IsShowDebugRay = true;
#endif

        #region ��������

        public float HorizontalAimingSpeed = 6f;
        public float VerticalAimingSpeed = 6f;
        public float FOVAimingSpeed = 1.0f;
        public float MaxVerticalAngle = 30f;
        public float MinVerticalAngle = -60f;
        public float TurnSmooth = 10f;

        #endregion

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods


        public virtual void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset)
        {
            targetPivotOffset = newPivotOffset;
            targetCamOffset = newCamOffset;
            isCustomOffset = true;
        }

        public virtual void ResetTargetOffsets()
        {
            targetPivotOffset = PivotOffset;
            targetCamOffset = CamOffset;
            targetPivotOffset.z = CamViewDistance.z;
            isCustomOffset = false;
        }

        public virtual void ResetYCamOffset()
        {
            targetCamOffset.y = CamOffset.y;
        }

        public virtual void SetYCamOffset(float y)
        {
            targetCamOffset.y = y;
        }

        public virtual void SetXCamOffset(float x)
        {
            targetCamOffset.x = x;
        }

        public virtual void SetMaxVerticalAngle(float angle)
        {
            targetMaxVerticalAngle = angle;
        }

        public virtual void ResetMaxVerticalAngle()
        {
            targetMaxVerticalAngle = MaxVerticalAngle;
        }


        /// <summary>
        /// ����������õ���ɫ���
        /// </summary>
        public virtual void ResetCameraToBack()
        {
            if(Reference && Target)
            {
                angleH = Reference.rotation.eulerAngles.y;
            }
        }


        /// <summary>
        /// ����Ŀ����󲢳�ʼ�������λ��
        /// </summary>
        /// <param name="tplayer"></param>
        public virtual void InitTarget(Transform tplayer)
        {
            Reference = tplayer;

            Init();
        }

        public override void Init()
        {
            base.Init();
            if (!Reference)
            {
#if UNITY_EDITOR || DEV_TEST
                Debug.Log($"{name}({GetType().Name}) Init fail:Null Reference.");
#endif
                return;
            }

            if (!Target)
            {
                Target = transform;
            }

            // Set up references and default values.
            smoothPivotOffset = PivotOffset;
            smoothCamOffset = CamOffset;
            _Camera.fieldOfView = DefaultFOV;

            ResetCameraToBack();

            ResetTargetOffsets();
            ResetMaxVerticalAngle();
            UpdateCameraPosition();
            UpdateCameraRotation();

#if UNITY_EDITOR || DEV_TEST
            if (CamOffset.y > 0)
                Debug.LogWarning("Vertical Cam Offset (Y) will be ignored during collisions!\n" +
                    "It is recommended to set all vertical offset in Pivot Offset.");
#endif
        }


        /// <summary>
        /// �������������תλ��
        /// </summary>
        public virtual void UpdateCameraRotation()
        {
            // Joystick:
            angleH += Mathf.Clamp(Input.GetAxis(XAxis), -1, 1) * 60 * HorizontalAimingSpeed * Time.deltaTime;
            angleV += Mathf.Clamp(Input.GetAxis(YAxis), -1, 1) * 60 * VerticalAimingSpeed * Time.deltaTime;

            // Set vertical movement limit.
            angleV = Mathf.Clamp(angleV, MinVerticalAngle, targetMaxVerticalAngle);

            // Set camera orientation.
            camYRotation = Quaternion.Euler(0, angleH, 0);
            aimRotation = Quaternion.Euler(-angleV, angleH, 0);
            Target.rotation = aimRotation;
        }

        /// <summary>
        /// ����������ĸ���λ��
        /// </summary>
        public virtual void UpdateCameraPosition()
        {
            // Test for collision with the environment based on current camera position.
            Vector3 baseTempPosition = Reference.position + camYRotation * targetPivotOffset;
            Vector3 noCollisionOffset = targetCamOffset;
            while (noCollisionOffset.magnitude >= 0.2f)
            {
                if (!IsCheckCollision)
                {
                    break;
                }

                if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset))
                {
                    _Camera.cullingMask = _OldMask;
                    break;
                }

                noCollisionOffset -= noCollisionOffset.normalized * 0.2f;
            }
            if (noCollisionOffset.magnitude < 1.0f)
            {
                noCollisionOffset = Vector3.zero;
                _Camera.cullingMask = TargetLayer;
            }
                
            // No intermediate position for custom offsets, go to 1st person.
            bool customOffsetCollision = isCustomOffset && noCollisionOffset.sqrMagnitude < targetCamOffset.sqrMagnitude;
            //bool customOffsetCollision =  noCollisionOffset.sqrMagnitude < targetCamOffset.sqrMagnitude;

            // Repostition the camera.
            smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, customOffsetCollision ? PivotOffset : targetPivotOffset, TurnSmooth * Time.deltaTime);
            smoothCamOffset = Vector3.Lerp(smoothCamOffset, customOffsetCollision ? Vector3.zero : noCollisionOffset, TurnSmooth * Time.deltaTime);
            targetPosition = Reference.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
            _TargetDirection = targetPosition - Target.position;

            Target.position = targetPosition;
        }

        public virtual void UpdateCameraFOV()
        {
            float delta = _TargetFOV - _Camera.fieldOfView;
            float absDelta = Mathf.Abs(delta);

            if (absDelta > 0.01)
            {
                _Camera.fieldOfView = delta > 0 ? _Camera.fieldOfView + FOVAimingSpeed : _Camera.fieldOfView - FOVAimingSpeed;
            }
            else if (_IsUpdateFOV)
            {
                _IsUpdateFOV = false;
            }
        }

        /// <summary>
        /// ע������¼�
        /// </summary>
        public override void RegsiterEvents()
        {
            RegsiterEvent(10003, OnFreeCameraSwitch, false, -1);
            RegsiterEvent(10004, OnLockCameraSwitch, false, -1);

            RegsiterEvent(11006, OnChangeCameraPositionCheck, false, -1);
            RegsiterEvent(11007, OnChangeCameraRotationCheck, false, -1);
            RegsiterEvent(11008, OnChangeCameraReference, false, -1);
            RegsiterEvent(11001, OnChangeCameraOffset, false, -1);
            RegsiterEvent(11002, OnResetCameraOffset, false, -1);
            RegsiterEvent(11003, OnChangeCameraCheckCollision, false, -1);
            RegsiterEvent(11004, OnChangeCameraTargetFOV, false, -1);
            RegsiterEvent(11005, OnResetCameraFOV, false, -1);
        }

        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="edata"></param>
        public virtual void OnFreeCameraSwitch(object edata)
        {
            bool IsFreeCam = (bool)edata;
        }

        /// <summary>
        /// ���������״̬�ı�
        /// </summary>
        /// <param name="edata"></param>
        public virtual void OnLockCameraSwitch(object edata)
        {
            bool tIsLockCam = (bool)edata;
            _IsLockCam = tIsLockCam;
        }

        /// <summary>
        /// �����λ��׷��״̬�ı�
        /// </summary>
        /// <param name="edata"></param>
        public virtual void OnChangeCameraPositionCheck(object edata)
        {
            bool tIsLockCam = (bool)edata;
            _IsCheckPosition = tIsLockCam;
        }


        /// <summary>
        /// �����λ��׷��״̬�ı�
        /// </summary>
        /// <param name="edata"></param>
        public virtual void OnChangeCameraRotationCheck(object edata)
        {
            bool tIsLockCam = (bool)edata;
            _IsCheckRotation = tIsLockCam;
        }

        /// <summary>
        /// �������׷��Ŀ��ı�
        /// </summary>
        /// <param name="edata"></param>
        public virtual void OnChangeCameraReference(object edata)
        {
            Transform newTransform = (Transform)edata;
            if(Reference!=newTransform)
            {
                InitTarget(newTransform);
                Init();
            }

        }

        public virtual void OnChangeCameraOffset(object edata)
        {
            Vector3 v3 = (Vector3)edata;

            SetTargetOffsets(PivotOffset, v3);
        }

        public virtual void OnResetCameraOffset(object edata)
        {
            ResetTargetOffsets();
        }

        public virtual void OnResetCameraFOV(object edata)
        {
            OnChangeCameraTargetFOV(DefaultFOV);
        }

        public virtual void OnChangeCameraCheckCollision(object edata)
        {
            IsCheckCollision = (bool)edata;
        }

        public virtual void OnChangeCameraTargetFOV(object edata)
        {
            _TargetFOV = (float)edata;
            float delta = Mathf.Abs(_TargetFOV - _Camera.fieldOfView);
            if (delta > 0.001 && !_IsUpdateFOV)
            {
                _IsUpdateFOV = true;
            }
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        public override void CheckInput()
        {
            base.CheckInput();

            _IsRotate = Input.GetButton(CamRotate);
            distanceZ = Input.GetAxis(ViewOffset);
        }

        public override void CheckState()
        {
            base.CheckState();

            if(Mathf.Abs(distanceZ) > 0)
            {
                CamViewDistance.z = Mathf.Clamp(targetCamOffset.z + distanceZ, CamViewDistance.x, CamViewDistance.y);
                targetCamOffset.z = CamViewDistance.z;
            }

            if (_IsLockCam && Reference)
            {
                if (_IsRotate && _IsCheckRotation)
                {
                    UpdateCameraRotation();
                }

                if(_IsCheckPosition || _IsRotate)
                {
                    UpdateCameraPosition();
                }        
            }

            if(_IsUpdateFOV)
            {
                UpdateCameraFOV();
            }

        }

        /// <summary>
        /// ˫�ؼ����ײ
        /// </summary>
        /// <param name="checkPos"></param>
        /// <returns></returns>
        protected virtual bool DoubleViewingPosCheck(Vector3 checkPos)
        {
            return ViewingPosCheck(checkPos) && ReverseViewingPosCheck(checkPos);
        }

        // Check for collision from camera to player.
        protected virtual bool ViewingPosCheck(Vector3 checkPos)
        {
            // Cast target and direction.
            Vector3 target = Reference.position + PivotOffset;
            Vector3 direction = target - checkPos;

#if UNITY_EDITOR
            // ��ײ�����ӻ�
            if(IsShowDebugRay)
            {
                Debug.DrawRay(checkPos, direction, Color.red, 1.0f);
            }          
#endif

            if (Physics.SphereCast(checkPos, 0.2f, direction, out RaycastHit hit, direction.magnitude, CollisionLayer))
            {
                // ... if it is not the player...
                if (hit.transform != Reference && !hit.transform.GetComponent<Collider>().isTrigger)
                {
#if UNITY_EDITOR
                    Debug.Log($"{name} ViewingPosCheck hit {hit.transform.name}");
#endif
                    // This position isn't appropriate.
                    return false;
                }
            }

            // If we haven't hit anything or we've hit the player, this is an appropriate position.
            return true;
        }

        protected virtual bool ReverseViewingPosCheck(Vector3 checkPos)
        {
            Vector3 origin = Reference.position + PivotOffset;
            Vector3 direction = checkPos - origin;
            if (Physics.SphereCast(origin, 0.2f, direction, out RaycastHit hit, direction.magnitude, CollisionLayer))
            {
                if (hit.transform != Reference && hit.transform != Target && !hit.transform.GetComponent<Collider>().isTrigger)
                {
#if UNITY_EDITOR
                    Debug.Log($"{name} ReverseViewingPosCheck hit {hit.transform.name}");
#endif
                    return false;
                }
            }
            return true;
        }


        #endregion --Protected Methods

        #region Unity Methods

        protected override void Start()
        {
            _Camera = GetComponent<Camera>();
            _OldMask = _Camera.cullingMask;
            base.Start();
        }

        #endregion --Unity Methods
    }
}
