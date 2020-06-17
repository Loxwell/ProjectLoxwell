using System.Collections.Generic;
using Int32 = System.Int32;
//using System.Net;
//using System.Runtime.InteropServices;
//using UnityEditor.ProjectWindowCallback;

namespace DataStructure.Collections
{
    public class HeapQueue<T> where T : System.IComparable<T>
    {
        private List<T> m_items;

        public T First
        {
            get
            {
                if (IsEmpty)
                    return default(T);
                return m_items[0];
            }
        }

        public int Count { get { return m_items.Count; } }

        public bool IsEmpty { get { return m_items == null || m_items.Count == 0; } }

        public bool Contians(T item) => m_items.Contains(item);

        public void Remove(T item) => m_items.Remove(item);
        
        public T Peek() => m_items[0];
        
        public HeapQueue()
        {
            m_items = new List<T>();
        }
        
        public void Clear()
        {
            m_items.Clear();
        }

        public void Destroy()
        {
            if (m_items != null)
            {
                m_items.Clear();
                m_items.TrimExcess();
            }

            m_items = null;
        }

        public void Push(T item)
        {
            //add item to end of tree to extend the list
            m_items.Add(item);

            //find correct position for new item.
            SiftDown(0, Count - 1);
        }
        public T Pop()
        {
            if (Count == 0)
                return default;

            //if there are more than 1 items, returned item will be first in tree.
            //then, add last item to front of tree, shrink the list
            //and find correct index in tree for first item.
            T item;
            int tail = Count - 1;
            
            T last = m_items[tail];
            m_items.RemoveAt(tail);

            if (Count > 0)
            {
                item = m_items[0];
                m_items[0] = last;
                SiftUp();
            }
            else
                item = last;

            return item;
        }

        private int Compare(T A, T B) => A.CompareTo(B);

        // Compare Priority
        private void SiftDown(int startPos, int pos)
        {
            if (pos >= Count || pos < 0 || startPos >= Count || startPos < 0)
                return;

            T newItem = m_items[pos];
            while (pos > startPos)
            {
                // find parent index in binary index
                System.Int32 parentPos = (pos - 1) >> 1;
                T parent = m_items[parentPos];

                //if new item precedes or equal to parent, pos is new item position
                if (Compare(parent, newItem) <= 0)
                    break;

                //else  move parent into pos, then repeat grand parent
                m_items[pos] = parent;
                pos = parentPos;
            }
            m_items[pos] = newItem;
        }

        void SiftUp()
        {
            Int32 endPos = m_items.Count;
            Int32 startPos = 0;

            //preserve the inserted item
            T newItem = m_items[0];
            Int32 childPos = 1;
            Int32 pos = 0;
            while (childPos < endPos)
            {
                // right branch
                Int32 rightPos = childPos + 1;

                // if right branch should precede left branch, move right branch up the tree
                if (rightPos < endPos && Compare(m_items[rightPos], m_items[childPos]) <= 0)
                    childPos = rightPos;

                m_items[pos] = m_items[childPos];
                pos = childPos;
                childPos = 2 * pos + 1;
            }
            m_items[pos] = newItem;
            SiftDown(startPos, pos);
        }
    }
}
