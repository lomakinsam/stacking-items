#region License and Information
/*****
*
* RingBuffer.cs
* 
* 2017.06.02 - first version 
* 
* Copyright (c) 2017 Bunny83
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to
* deal in the Software without restriction, including without limitation the
* rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
* sell copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
* IN THE SOFTWARE.
* 
*****/
#endregion License and Information
using System.Collections;
using System.Collections.Generic;

namespace B83.Collections
{
    public class RingBuffer<T> : ICollection<T>, IList<T>
    {
        private T[] m_Data;
        private int m_Count;
        private int m_Read;
        private int m_Write;
        private int m_Version = 0;

        public int Count { get { return m_Count; } }
        public bool IsReadOnly { get { return false; } }

        public T this[int index]
        {
            get
            {
                if (m_Count == 0)
                    throw new System.InvalidOperationException("RingBuffer.this[]:: buffer is empty");
                if (index < 0 || index >= m_Count)
                    throw new System.ArgumentOutOfRangeException("index", index, "RingBuffer.this[index] is out of range (0 .. Count-1)");
                index = (m_Read + index) % m_Data.Length;
                return m_Data[index];
            }
            set
            {
                if (m_Count == 0)
                    throw new System.InvalidOperationException("RingBuffer.this[]:: buffer is empty");
                if (index < 0 || index >= m_Count)
                    throw new System.ArgumentOutOfRangeException("index", index, "RingBuffer.this[index] is out of range (0 .. Count-1)");
                index = (m_Read + index) % m_Data.Length;
                m_Data[index] = value;
            }
        }

        public RingBuffer(int aSize)
        {
            m_Data = new T[aSize];
            m_Count = 0;
            m_Read = 0;
            m_Write = 0;
        }

        public T ReadAndRemoveNext()
        {
            if (m_Count == 0)
                throw new System.InvalidOperationException("Read not possible, RingBuffer is empty");
            T res = m_Data[m_Read];
            m_Data[m_Read] = default(T);
            m_Read = (m_Read + 1) % m_Data.Length;
            m_Count--;
            m_Version++;
            return res;
        }
        public T ReadAndRemoveNext(T aDefaultValue)
        {
            if (m_Count == 0)
                return aDefaultValue;
            T res = m_Data[m_Read];
            m_Read = (m_Read + 1) % m_Data.Length;
            m_Count--;
            m_Version++;
            return res;
        }

        public void Add(T aData)
        {
            m_Data[m_Write] = aData;
            m_Write = (m_Write + 1) % m_Data.Length;
            if (m_Count == m_Data.Length)
                m_Read = (m_Read + 1) % m_Data.Length;
            else
                m_Count++;
            m_Version++;
        }

        public void Clear()
        {
            for (int i = 0; i < m_Data.Length; i++)
                m_Data[i] = default(T);
            m_Read = 0;
            m_Write = 0;
            m_Count = 0;
            m_Version++;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < m_Data.Length; i++)
                if (EqualityComparer<T>.Default.Equals(item, m_Data[i]))
                    return true;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new System.ArgumentNullException("RingBuffer.CopyTo::Passed array is null");
            if (arrayIndex >= array.Length)
                throw new System.ArgumentOutOfRangeException("arrayIndex", "RingBuffer.CopyTo::Passed index is out of range");
            int space = array.Length - arrayIndex;
            if (space < m_Count)
                throw new System.ArgumentException("RingBuffer.CopyTo::Passed array is too small (" + space + " while " + m_Count + " is needed)");
            int a = System.Math.Min(m_Count, m_Data.Length - m_Read);
            System.Array.Copy(m_Data, m_Read, array, arrayIndex, a);
            if (a < m_Count)
            {
                arrayIndex += a;
                System.Array.Copy(m_Data, 0, array, arrayIndex, m_Count - a);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            int version = m_Version;
            for (int i = 0; i < m_Count; i++)
            {
                int index = (m_Read + i) % m_Data.Length;
                yield return m_Data[index];
                if (m_Version != version)
                    throw new System.InvalidOperationException("RingBuffer.IEumerator::The data has changed so the enumeration can't be continued");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < m_Count; i++)
            {
                int index = (m_Read + i) % m_Data.Length;
                if (EqualityComparer<T>.Default.Equals(m_Data[i], item))
                    return i;
            }
            return -1;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= m_Count)
                throw new System.ArgumentOutOfRangeException("index", index, "RingBuffer.RemoveAt::index out of bounds");
            int toIndex = (m_Read + index) % m_Data.Length;
            for (int i = index; i < m_Count; i++)
            {
                int fromIndex = (toIndex + 1) % m_Data.Length;
                m_Data[toIndex] = m_Data[fromIndex];
                toIndex = fromIndex;
            }
            m_Data[toIndex] = default(T);
        }
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
                return false;
            RemoveAt(index);
            return true;
        }

        public void Insert(int index, T item)
        {
            throw new System.NotSupportedException();
        }

    }
}