using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public interface IScrollRectListItem
    {
        public void Init(object data);
    }

    public class ScrollRectListItem : MonoBehaviour,IScrollRectListItem
    {
        public ScrollRectList ParentList;

        public virtual void Init(object data)
        {

        }
    }
}
