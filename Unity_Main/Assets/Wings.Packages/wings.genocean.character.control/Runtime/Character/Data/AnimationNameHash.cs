using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GenOcean.Common;

namespace GenOcean.Character.Data
{
    /// <summary>
    /// 记录了一些人形角色常用的动画参数名和参数名的Hash
    /// </summary>
    public class AnimationNameHash:ManagerBase
    {
        public static string MoveSpeed = "MoveSpeed";
        public static string Horizontal = "Horizontal";
        public static string Vertical = "Vertical";
        public static string Grounded = "Grounded";
        public static string Jump = "Jump";
        public static string Fly = "Fly";
        public static string Aim = "Aim";

        public int MoveSpeedHash = Animator.StringToHash(MoveSpeed);
        public int HorizontalHash = Animator.StringToHash(Horizontal);
        public int VerticalHash = Animator.StringToHash(Vertical);
        public int GroundedHash = Animator.StringToHash(Grounded);
        public int JumpHash = Animator.StringToHash(Jump);
        public int FlyHash = Animator.StringToHash(Fly);
        public int AimHash = Animator.StringToHash(Aim);
    }

    public class SingleAnimationHashManager : SingletonManagerBase<AnimationNameHash>
    {
        public static int MoveSpeedHash
        {
            get
            {
                return Instance.MoveSpeedHash;
            }
        }

        public static int HorizontalHash
        {
            get
            {
                return Instance.MoveSpeedHash;
            }
        }

        public static int VerticalHash
        {
            get
            {
                return Instance.VerticalHash;
            }
        }

        public static int GroundedHash
        {
            get
            {
                return Instance.GroundedHash;
            }
        }

        public static int JumpHash
        {
            get
            {
                return Instance.JumpHash;
            }
        }

        public static int FlyHash
        {
            get
            {
                return Instance.FlyHash;
            }
        }

        public static int AimHash
        {
            get
            {
                return Instance.AimHash;
            }
        }
    }

}
