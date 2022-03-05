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
        /// ����
        /// </summary>
        protected Stack<T> _MainPool = new Stack<T>();

        #endregion --Protected Fields

        #region Public Fields

        #endregion --Public Fields

        #region Private Fields
        #endregion --Private Fields

        #region Public Methods

        /// <summary>
        /// ������һ��������ӵ�����
        /// <para>�����ʹ��������������������������Ӧ�ñ��û���������</para>
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
        /// �������л�ȡһ������
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
                // �����Ѿ���ȡ�յ�ʱ�򣬴����¶���
                obj = CreateNewPoolObject();
                obj.Pool = this;
            }

            obj.Active();
            return obj;
        }

        /// <summary>
        /// ����һ���¶���
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
