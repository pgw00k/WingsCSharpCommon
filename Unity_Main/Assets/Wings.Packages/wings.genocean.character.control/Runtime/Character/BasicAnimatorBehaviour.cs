/*
 * FileName:    BasicAnimatorBehaviour2
 * Author:      Wings
 * CreateTime:  2021_12_30
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GenOcean.GameFrame;

namespace GenOcean.Character
{
    /// <summary>
    /// ��������Ϊ�������
    /// <para>һ����˵����Ϊһ���Ͷ�����״̬����ƥ��</para>
    /// </summary>
    public class BasicAnimatorBehaviour : BasicBehaviour
    {
        #region Protected Fields

        protected AnimatorHandler _Anim;

        #endregion --Protected Fields

        #region Public Fields

        public AnimatorHandler Anim { get { return _Anim; } }

        public Animator Animator { get { return _Anim.Animator; } }

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods
        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods
        #endregion --Protected Methods

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _Anim = GetComponent<AnimatorHandler>();
        }

        #endregion --Unity Methods
    }
}
