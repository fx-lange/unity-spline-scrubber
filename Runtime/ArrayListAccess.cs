using System;
using System.Collections;
using System.Collections.Generic;

namespace SplineScrubber
{
    public class ArrayListAccess<T> : IList<T>
        //ArrayViewList ArrayAccessList DynamicArraySegments
    {
        private T[] _items;

        public ArrayListAccess()
        {
            _items = System.Array.Empty<T>();
        }

        public ArrayListAccess(int capacity)
        {
            _items = new T[capacity];
        }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public T[] Array => new ArraySegment<T>(_items, 0, Count).Array;

        public T[] ArrayView(int index, int count)
        {
            if (index < 0 || index + count > Count)
            {
                throw new IndexOutOfRangeException();
            }

            return new ArraySegment<T>(_items, index, count).Array;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return _items[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                _items[index] = value;
            }
        }

        public void Add(T item)
        {
            if (Count == _items.Length)
            {
                System.Array.Resize(ref _items, (_items.Length == 0) ? 4 : _items.Length * 2);
            }

            _items[Count++] = item;
        }

        public void Clear()
        {
            // System.Array.Clear(_items, 0, Count);
            Count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex + Count > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = _items[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _items[i];
            }
        }

        public int IndexOf(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(_items[i], item))
                    return i;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > Count)
                throw new IndexOutOfRangeException();

            if (Count == _items.Length)
            {
                System.Array.Resize(ref _items, (_items.Length == 0) ? 4 : _items.Length * 2);
            }

            System.Array.Copy(_items, index, _items, index + 1, Count - index);
            _items[index] = item;
            Count++;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
                return false;

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            System.Array.Copy(_items, index + 1, _items, index, Count - index - 1);
            _items[--Count] = default(T);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}