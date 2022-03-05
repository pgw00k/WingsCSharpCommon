using System.Collections;
using System.Collections.Generic;

namespace GenOcean.Common
{
    /// <summary>
    /// C sharp 单例类的接口
    /// <para>所有原生单例基类都应该继承此接口</para>
    /// </summary>
    public interface IManagerBase
    {
        /// <summary>
        /// 单例初始化
        /// </summary>
        void Init();

    }

    /// <summary>
    /// 所有逻辑类单例的基类
    /// <para>不继承自MONO类，使用C Sharp 原生类</para>
    /// <para>子类只改重写Init来用作初始化，而不应改有任何有参构造</para>
    /// <para>单例类的资源在运行时会一直存在，不会释放</para>
    /// </summary>
    public abstract class ManagerBase: IManagerBase
    {
        public virtual void Init() 
        {
        }
    }

    /// <summary>
    /// 所有逻辑类单例的基类
    /// <para>不继承自MONO类，使用C Sharp 原生类</para>
    /// <para>子类需要都带有 Single 前缀 和 Manager 字样</para>
    /// </summary>
    public class SingletonManagerBase<T> : ManagerBase where T : ManagerBase, new()
    {
        /// <summary>
        /// 存储单例的指针
        /// </summary>
        protected volatile static T _Instance = null;

        /// <summary>
        /// 用以保证线程安全的锁
        /// </summary>
        protected static readonly object _LockObject = new object();

        /// <summary>
        /// 单例入口
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_LockObject)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new T();
                            _Instance.Init();
                        }
                    }
                }
                return _Instance;
            }
        }

    }
}
