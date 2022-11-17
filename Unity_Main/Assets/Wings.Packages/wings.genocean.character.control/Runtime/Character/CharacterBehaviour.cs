/*
 * FileName:    CharacterBehaviour
 * Author:      Wings
 * CreateTime:  2021_12_30
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GenOcean.Common;
using GenOcean.GameFrame;

namespace GenOcean.Character
{
    /// <summary>
    /// ����Ļ���
    /// <para>��������ϵͳ/para>
    /// </summary>
    [DefaultExecutionOrder(-90)]
    public class CharacterBehaviour : BasicAnimatorBehaviour
    {
        #region Protected Fields

        protected Rigidbody _Rig;

        /// <summary>
        /// ��ʱ������������
        /// </summary>
        protected int _TempLockInputCount = 0;

        protected bool _PrevCheckInput = true;

        #endregion --Protected Fields

        #region Public Fields

        public Rigidbody Rig { get { return _Rig; } }

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        /// <summary>
        /// ע������¼�
        /// </summary>
        public override void RegsiterEvents()
        {
            RegsiterEvent(10002, LockInputState, false, -1);
            RegsiterEvent(10012, TempLockInputState, false, -1);
        }

        /// <summary>
        /// �Ƿ���������
        /// </summary>
        /// <param name="edata"></param>
        public virtual void LockInputState(object edata)
        {
            IsCheckInput = !(bool)edata;
            if(!IsCheckInput)
            {
                UncheckInput();
            }
        }

        /// <summary>
        /// �Ƿ���ʱ��������
        /// </summary>
        /// <param name="edata"></param>
        public virtual void TempLockInputState(object edata)
        {
            bool _IsLock = (bool)edata;
            if(_IsLock)
            {
                _TempLockInputCount++;
                UncheckInput();         
            }
            else
            {
                _TempLockInputCount--;
                if(_TempLockInputCount <=0 )
                {
                    _TempLockInputCount = 0;
                }
            }
        }

        /// <summary>
        /// �Ƴ����崹ֱ�����ϵļ��ٶ�
        /// </summary>
        public virtual void RemoveVerticalVelocity()
        {
            Vector3 horizontalVelocity = _Rig.velocity;
            horizontalVelocity.y = 0;
            _Rig.velocity = horizontalVelocity;
        }

        public override bool IsNeedCheckInput()
        {
            return base.IsNeedCheckInput() && _TempLockInputCount <= 0 ;
        }



        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        #endregion --Protected Methods

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _Rig = GetComponent<Rigidbody>();
        }

        #endregion --Unity Methods
    }
}
