/*
 * FileName:    Pawn
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
    public interface ICheckInput
    {
        public void CheckInput();
        public void UncheckInput();
    }

    public interface IPawn: ICheckInput
    {
        public void CheckState();
        public void CheckAnim();
    }

    /// <summary>
    /// ���Բ����Ķ������
    /// </summary>
    public class Pawn : EventMonoComponent, IPawn
    {
        #region Protected Fields
        #endregion --Protected Fields

        #region Public Fields

        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool IsCheckInput = true;

        /// <summary>
        /// �Ƿ�ˢ�¶���
        /// </summary>
        public bool IsCheckAnim = true;

        /// <summary>
        /// �Ƿ�ˢ��״̬
        /// </summary>
        public bool IsCheckState = true;

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public override void Init()
        {
            base.Init();
        }

        public virtual bool IsNeedCheckInput()
        {
            return IsCheckInput;
        }

        public virtual bool IsNeedCheckState()
        {
            return IsCheckState;
        }

        public virtual bool IsNeedIsCheckAnim()
        {
            return IsCheckAnim;
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        protected virtual void LocalCheck()
        {
            if (IsNeedCheckInput())
            {
                CheckInput();
            }

            if (IsNeedCheckState())
            {
                CheckState();
            }

            if (IsNeedIsCheckAnim())
            {
                CheckAnim();
            }
        }

        /// <summary>
        /// ��ⰴ������
        /// </summary>
        public virtual void CheckInput()
        {
        }

        /// <summary>
        /// ���״̬
        /// </summary>
        public virtual void CheckState()
        {
        }

        /// <summary>
        /// ��⶯��״̬
        /// </summary>
        public virtual void CheckAnim()
        {
        }

        /// <summary>
        /// ��ϲ���ʱ����Ӧ����
        /// </summary>
        public virtual void UncheckInput()
        {
        }

        #endregion --Protected Methods

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
        }
        protected virtual void Update()
        {
            LocalCheck();
        }

        #endregion --Unity Methods
    }
}
