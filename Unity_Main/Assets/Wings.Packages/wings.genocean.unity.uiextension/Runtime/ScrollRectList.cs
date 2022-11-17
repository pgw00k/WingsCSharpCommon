using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityEngine.UI
{
    public class ScrollRectList : ScrollRect, IList<IScrollRectListItem>
    {
        public ScrollRectListItem Template;

        [SerializeField]
        public List<IScrollRectListItem> Items = new List<IScrollRectListItem>();

        #region IList Interface


        public int Count { get => Items.Count; }

        public bool IsReadOnly { get => false; }

        public IScrollRectListItem this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                Items[index] = value;
            }
        }


        public virtual int IndexOf(IScrollRectListItem item)
        {
            return Items.IndexOf(item);
        }

        public virtual void Insert(int index, IScrollRectListItem item)
        {
            Items.Insert(index, item);
        }

        public virtual void RemoveAt(int index)
        {
            Destroy((Items[index] as ScrollRectListItem).gameObject);
            Items.RemoveAt(index);
        }

        public virtual void Add(IScrollRectListItem item)
        {
            Items.Add(item);
        }

        public virtual bool Contains(IScrollRectListItem item)
        {
            return Items.Contains(item);
        }

        public virtual void CopyTo(IScrollRectListItem[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(IScrollRectListItem item)
        {
            Destroy((item as ScrollRectListItem).gameObject);
            return Items.Remove(item);
        }

        public virtual IEnumerator<IScrollRectListItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion

        public virtual void AddObject(object data)
        {
            GameObject NewGo = GameObject.Instantiate(Template.gameObject);
            NewGo.transform.SetParent(Template.transform.parent, false);
            NewGo.transform.localPosition = Template.transform.localPosition;
            NewGo.transform.localRotation = Template.transform.localRotation;
            NewGo.transform.localScale = Template.transform.localScale;
            NewGo.SetActive(true);

            ScrollRectListItem item = NewGo.GetComponent<ScrollRectListItem>();
            item.Init(data);
            Add(item);
        }
        public virtual void Clear()
        {
            while(Items.Count > 0)
            {
                Destroy((Items[0] as ScrollRectListItem).gameObject);
                Items.RemoveAt(0);
            }
        }

        public virtual void UpdateContentHeight()
        {

        }

        public virtual void Awake()
        {
            base.Awake();
            if(Template)
            {
                Template.ParentList = this;
            }
        }
    }
}
