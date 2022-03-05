/*
 * 
 * FileName:    PoolManager
 * Author:      Wings
 * CreateTime:  2021_12_29
 * 
*/

using System.Collections;
using System.Collections.Generic;

namespace GenOcean.Common
{
    public interface IPoolManager
    {
        public void PushPool(IPoolObject obj);
        //public IPoolObject Get();
        //public IPoolObject CreateNewPoolObject();
    }

    public interface IPoolManager<T>: IPoolManager
    {
        public T Get();
        public T CreateNewPoolObject();
    }

    public interface IPoolObject
    {
        public IPoolManager Pool { get; set; }
        public void Active();
        public void Deactive();
        public bool IsActive { get; }
    }

    public class PoolManager<T> : IPoolManager<T>
        where T : IPoolObject
    {
        #region Protected Fields

        /// <summary>
        /// 主池
        /// </summary>
        protected Stack<T> _MainPool = new Stack<T>();

        #endregion --Protected Fields

        #region Public Fields

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        /// <summary>
        /// 主动将一个对象添加到池中
        /// <para>请谨慎使用这个函数，理论上这个函数不应该被用户主动调用</para>
        /// </summary>
        /// <param name="obj"></param>
        public virtual void PushPool(IPoolObject obj)
        {
            if (obj.IsActive)
            {
                obj.Deactive();
            }
            _MainPool.Push((T)obj);
        }

        /// <summary>
        /// 从主池中获取一个对象
        /// </summary>
        /// <returns></returns>
        public virtual T Get()
        {
            T obj = default(T);
            if (_MainPool.Count > 0)
            {
                obj = _MainPool.Pop();
            }else
            {
                // 主池已经被取空的时候，创建新对象
                obj = CreateNewPoolObject();
                obj.Pool = this;
            }

            obj.Active();
            return obj;
        }

        /// <summary>
        /// 创建一个新对象
        /// </summary>
        /// <returns></returns>
        public virtual T CreateNewPoolObject()
        {
            return default(T);
        }

        #endregion --Public Methods

        #region Private Methods
        #endregion --Public Methods

        #region Protected Methods

        #endregion --Protected Methods
    }
}
