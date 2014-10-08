using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ModulusFE.SL.Utils
{
    ///<summary>
    ///</summary>
    public static class ThrowHelper
    {
        ///<summary>
        ///</summary>
        ///<param name="message"></param>
        ///<exception cref="NotImplementedException"></exception>
        public static void ThrowNotSupportedException(string message)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<exception cref="ArgumentNullException"></exception>
        public static void ThrowArgumentNullException(string key)
        {
            throw new ArgumentNullException(key);
        }

        ///<summary>
        ///</summary>
        ///<param name="argument"></param>
        ///<exception cref="ArgumentException"></exception>
        public static void ThrowArgumentException(string argument)
        {
            throw new ArgumentException(argument);
        }

        ///<summary>
        ///</summary>
        ///<param name="operation"></param>
        ///<exception cref="InvalidOperationException"></exception>
        public static void ThrowInvalidOperationException(string operation)
        {
            throw new InvalidOperationException(operation);
        }

        ///<summary>
        ///</summary>
        ///<param name="paramName"></param>
        ///<param name="message"></param>
        ///<exception cref="ArgumentOutOfRangeException"></exception>
        public static void ThrowArgumentOutOfRangeException(string paramName, string message)
        {
            throw new ArgumentOutOfRangeException(paramName, message);
        }

        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<param name="type"></param>
        ///<exception cref="Exception"></exception>
        public static void ThrowWrongKeyTypeArgumentException(object key, Type type)
        {
            throw new Exception(string.Format("Wrong key type argument"));
        }

        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        ///<param name="type"></param>
        ///<exception cref="Exception"></exception>
        public static void ThrowWrongValueTypeArgumentException(object value, Type type)
        {
            throw new Exception(string.Format("Wrong value type argument"));
        }

        ///<summary>
        ///</summary>
        ///<exception cref="NotImplementedException"></exception>
        public static void ThrowKeyNotFoundException()
        {
            throw new NotImplementedException();
        }
    }

    ///<summary>
    ///</summary>
    internal static class ExceptionResource
    {
        ///<summary>
        ///</summary>
        public const string ArgumentOutOfRange_SmallCapacity = "";
        ///<summary>
        ///</summary>
        public const string Arg_NonZeroLowerBound = "";
        ///<summary>
        ///</summary>
        public const string Arg_ArrayPlusOffTooSmall = "";
        public const string ArgumentOutOfRange_NeedNonNegNum = "";
        public const string ArgumentOutOfRange_Index = "";
        public const string Argument_AddingDuplicate = "";
        public const string ArgumentOutOfRange_NeedNonNegNumRequired = "";
        public const string InvalidOperation_EnumOpCantHappen = "";
        public const string InvalidOperation_EnumFailedVersion = "";
        public const string Argument_InvalidArrayType = "";
        public const string Arg_RankMultiDimNotSupported = "";
        public const string NotSupported_SortedListNestedWrite = "";
    }

    ///<summary>
    /// Represents a collection of key/value pairs that are sorted by key based on the associated <see cref="IComparer{T}"/> implementation
    ///</summary>
    ///<typeparam name="TKey"></typeparam>
    ///<typeparam name="TValue"></typeparam>
    public class SortedList<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        // Fields
        private int _size;
        private object _syncRoot;
        private readonly IComparer<TKey> comparer;
        private static readonly TKey[] emptyKeys;
        private static readonly TValue[] emptyValues;
        private KeyList keyList;
        private TKey[] keys;
        private ValueList valueList;
        private TValue[] values;
        private int version;

        // Methods
        static SortedList()
        {
            emptyKeys = new TKey[0];
            emptyValues = new TValue[0];
        }

        ///<summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}"/> class that is empty, has the default initial capacity, and uses the default <see cref="IComparer{T}"/>>).
        ///</summary>
        public SortedList()
        {
            keys = emptyKeys;
            values = emptyValues;
            _size = 0;
            comparer = Comparer<TKey>.Default;
        }

        ///<summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}"/> class that is empty, has the default initial capacity, and uses the specified <see cref="IComparer{T}"/>). 
        ///</summary>
        ///<param name="comparer"></param>
        public SortedList(IComparer<TKey> comparer)
            : this()
        {
            if (comparer != null)
            {
            }
        }

        ///<summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}"/> class that contains elements copied from the specified 
        /// <see cref="IDictionary{TKey,TValue}"/>, has sufficient capacity to accommodate the number of elements copied, and uses 
        /// the default <see cref="IComparer{T}"/>.
        ///</summary>
        ///<param name="dictionary"></param>
        public SortedList(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        {
        }

        ///<summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}"/> class that is empty, has the specified 
        /// initial capacity, and uses the default <see cref="IComparer{T}"/>.
        ///</summary>
        ///<param name="capacity"></param>
        public SortedList(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("capacity", ExceptionResource.ArgumentOutOfRange_NeedNonNegNumRequired);
            }
            keys = new TKey[capacity];
            values = new TValue[capacity];
            comparer = Comparer<TKey>.Default;
        }

        ///<summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}"/> class that contains elements copied 
        /// from the specified <see cref="IDictionary{TKey,TValue}"/>, has sufficient capacity to accommodate the number 
        /// of elements copied, and uses the specified <see cref="IComparer{T}"/>.
        ///</summary>
        ///<param name="dictionary"></param>
        ///<param name="comparer"></param>
        public SortedList(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
            : this((dictionary != null) ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
            {
                ThrowHelper.ThrowArgumentNullException("dictionary");
            }
            dictionary.Keys.CopyTo(keys, 0);
            dictionary.Values.CopyTo(values, 0);
            Array.Sort(keys, values, comparer);
            _size = dictionary.Count;
        }

        ///<summary>
        /// Initializes a new instance of the <see cref="IDictionary{TKey,TValue}"/> class that is empty, has the specified initial capacity, 
        /// and uses the specified <see cref="IComparer{T}"/>.
        ///</summary>
        ///<param name="capacity"></param>
        ///<param name="comparer"></param>
        public SortedList(int capacity, IComparer<TKey> comparer)
            : this(comparer)
        {
            Capacity = capacity;
        }

        /// <summary>
        /// Adds an element with the specified key and value into the <see cref="SortedList{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("key");
            }
            int num = Array.BinarySearch(keys, 0, _size, key, comparer);
            if (num >= 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_AddingDuplicate);
            }
            Insert(~num, key, value);
        }

        /// <summary>
        /// Removes all elements from the <see cref="SortedList{TKey,TValue}"/>
        /// </summary>
        public void Clear()
        {
            version++;
            Array.Clear(keys, 0, _size);
            Array.Clear(values, 0, _size);
            _size = 0;
        }

        /// <summary>
        /// Determines whether the <see cref="SortedList{TKey,TValue}"/> contains a specific key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return (IndexOfKey(key) >= 0);
        }

        ///<summary>
        /// Determines whether the <see cref="SortedList{TKey,TValue}"/> contains a specific value.
        ///</summary>
        ///<param name="value"></param>
        ///<returns></returns>
        public bool ContainsValue(TValue value)
        {
            return (IndexOfValue(value) >= 0);
        }

        private void EnsureCapacity(int min)
        {
            int num = (keys.Length == 0) ? 4 : (keys.Length * 2);
            if (num < min)
            {
                num = min;
            }
            Capacity = num;
        }

        private TValue GetByIndex(int index)
        {
            if ((index < 0) || (index >= _size))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("index", ExceptionResource.ArgumentOutOfRange_Index);
            }
            return values[index];
        }

        ///<summary>
        /// Returns an enumerator that iterates through the <see cref="SortedList{TKey,TValue}"/>.
        ///</summary>
        ///<returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this, 1);
        }

        private TKey GetKey(int index)
        {
            if ((index < 0) || (index >= _size))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("index", ExceptionResource.ArgumentOutOfRange_Index);
            }
            return keys[index];
        }

        private KeyList GetKeyListHelper()
        {
            return keyList ?? (keyList = new KeyList(this));
        }

        private ValueList GetValueListHelper()
        {
            return valueList ?? (valueList = new ValueList(this));
        }

        ///<summary>
        /// Searches for the specified key and returns the zero-based index within the entire <see cref="SortedList{TKey,TValue}"/>
        ///</summary>
        ///<param name="key"></param>
        ///<returns></returns>
        public int IndexOfKey(TKey key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("key");
            }
            int num = Array.BinarySearch(keys, 0, _size, key, comparer);
            if (num < 0)
            {
                return -1;
            }
            return num;
        }

        ///<summary>
        /// Searches for the specified value and returns the zero-based index of the first occurrence 
        /// within the entire <see cref="SortedList{TKey,TValue}"/>.
        ///</summary>
        ///<param name="value"></param>
        ///<returns></returns>
        public int IndexOfValue(TValue value)
        {
            return Array.IndexOf(values, value, 0, _size);
        }

        private void Insert(int index, TKey key, TValue value)
        {
            if (_size == keys.Length)
            {
                EnsureCapacity(_size + 1);
            }
            if (index < _size)
            {
                Array.Copy(keys, index, keys, index + 1, _size - index);
                Array.Copy(values, index, values, index + 1, _size - index);
            }
            keys[index] = key;
            values[index] = value;
            _size++;
            version++;
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("key");
            }
            return (key is TKey);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="SortedList{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            int index = IndexOfKey(key);
            if (index >= 0)
            {
                RemoveAt(index);
            }
            return (index >= 0);
        }

        ///<summary>
        /// Removes the element at the specified index of the <see cref="SortedList{TKey,TValue}"/>.
        ///</summary>
        ///<param name="index"></param>
        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= _size))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("index", ExceptionResource.ArgumentOutOfRange_Index);
            }
            _size--;
            if (index < _size)
            {
                Array.Copy(keys, index + 1, keys, index, _size - index);
                Array.Copy(values, index + 1, values, index, _size - index);
            }
            keys[_size] = default(TKey);
            values[_size] = default(TValue);
            version++;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = IndexOfKey(keyValuePair.Key);
            return ((index >= 0) && EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value));
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException("array");
            }
            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("arrayIndex", ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if ((array.Length - arrayIndex) < Count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            for (int i = 0; i < Count; i++)
            {
                KeyValuePair<TKey, TValue> pair = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
                array[arrayIndex + i] = pair;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = IndexOfKey(keyValuePair.Key);
            if ((index >= 0) && EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value))
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, 1);
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException("array");
            }
            if (array.Rank != 1)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }
            if (array.GetLowerBound(0) != 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            }
            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("arrayIndex", ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if ((array.Length - arrayIndex) < Count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            KeyValuePair<TKey, TValue>[] pairArray = array as KeyValuePair<TKey, TValue>[];
            if (pairArray != null)
            {
                for (int i = 0; i < Count; i++)
                {
                    pairArray[i + arrayIndex] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
                }
            }
            else
            {
                object[] objArray = array as object[];
                if (objArray == null)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
                try
                {
                    for (int j = 0; j < Count; j++)
                    {
                        objArray[j + arrayIndex] = new KeyValuePair<TKey, TValue>(keys[j], values[j]);
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
            }
        }

        void IDictionary.Add(object key, object value)
        {
            VerifyKey(key);
            VerifyValueType(value);
            Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return (IsCompatibleKey(key) && ContainsKey((TKey)key));
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, 1);
        }

        ///<summary>
        /// Sets the capacity to the actual number of elements in the <see cref="SortedList{TKey,TValue}"/>, if that 
        /// number is less than 90 percent of current capacity
        ///</summary>
        public void TrimExcess()
        {
            int num = (int)(keys.Length * 0.9);
            if (_size < num)
            {
                Capacity = _size;
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = IndexOfKey(key);
            if (index >= 0)
            {
                value = values[index];
                return true;
            }
            value = default(TValue);
            return false;
        }

        private static void VerifyKey(object key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("key");
            }
            if (!(key is TKey))
            {
                ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
            }
        }

        private static void VerifyValueType(object value)
        {
            if (!(value is TValue) && ((value != null) || typeof(TValue).IsValueType))
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
            }
        }


        ///<summary>
        /// Gets or sets the number of elements that the <see cref="SortedList{TKey,TValue}"/> can contain.
        ///</summary>
        public int Capacity
        {
            get
            {
                return keys.Length;
            }
            set
            {
                if (value == keys.Length) return;

                if (value < _size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value", ExceptionResource.ArgumentOutOfRange_SmallCapacity);
                }
                if (value > 0)
                {
                    TKey[] destinationArray = new TKey[value];
                    TValue[] localArray2 = new TValue[value];
                    if (_size > 0)
                    {
                        Array.Copy(keys, 0, destinationArray, 0, _size);
                        Array.Copy(values, 0, localArray2, 0, _size);
                    }
                    keys = destinationArray;
                    values = localArray2;
                }
                else
                {
                    keys = emptyKeys;
                    values = emptyValues;
                }
            }
        }

        ///<summary>
        /// Gets the <see cref="IComparer{T}"/> for the sorted list. 
        ///</summary>
        public IComparer<TKey> Comparer
        {
            get
            {
                return comparer;
            }
        }

        /// <summary>
        ///  Gets the number of key/value pairs contained in the <see cref="SortedList{TKey,TValue}"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return _size;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                int index = IndexOfKey(key);
                if (index >= 0)
                {
                    return values[index];
                }
                ThrowHelper.ThrowKeyNotFoundException();
                return default(TValue);
            }
            set
            {
                if (key == null)
                {
                    ThrowHelper.ThrowArgumentNullException("key");
                }
                int index = Array.BinarySearch(keys, 0, _size, key, comparer);
                if (index >= 0)
                {
                    values[index] = value;
                    version++;
                }
                else
                {
                    Insert(~index, key, value);
                }
            }
        }

        ///<summary>
        /// Gets a collection containing the keys in the <see cref="SortedList{TKey,TValue}"/>.
        ///</summary>
        public IList<TKey> Keys
        {
            get
            {
                return GetKeyListHelper();
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return GetKeyListHelper();
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return GetValueListHelper();
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    int index = IndexOfKey((TKey)key);
                    if (index >= 0)
                    {
                        return values[index];
                    }
                }
                return null;
            }
            set
            {
                VerifyKey(key);
                VerifyValueType(value);
                this[(TKey)key] = (TValue)value;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return GetKeyListHelper();
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return GetValueListHelper();
            }
        }

        ///<summary>
        /// Gets a collection containing the values in the <see cref="SortedList{TKey,TValue}"/>.
        ///</summary>
        public IList<TValue> Values
        {
            get
            {
                return GetValueListHelper();
            }
        }

        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly SortedList<TKey, TValue> _sortedList;
            private TKey key;
            private TValue value;
            private int index;
            private readonly int version;
            private readonly int getEnumeratorRetType;
            internal Enumerator(SortedList<TKey, TValue> sortedList, int getEnumeratorRetType)
            {
                _sortedList = sortedList;
                index = 0;
                version = _sortedList.version;
                this.getEnumeratorRetType = getEnumeratorRetType;
                key = default(TKey);
                value = default(TValue);
            }

            public void Dispose()
            {
                index = 0;
                key = default(TKey);
                value = default(TValue);
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if ((index == 0) || (index == (_sortedList.Count + 1)))
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return key;
                }
            }
            public bool MoveNext()
            {
                if (version != _sortedList.version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                if (index < _sortedList.Count)
                {
                    key = _sortedList.keys[index];
                    value = _sortedList.values[index];
                    index++;
                    return true;
                }
                index = _sortedList.Count + 1;
                key = default(TKey);
                value = default(TValue);
                return false;
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if ((index == 0) || (index == (_sortedList.Count + 1)))
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return new DictionaryEntry(key, value);
                }
            }
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return new KeyValuePair<TKey, TValue>(key, value);
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    if ((index == 0) || (index == (_sortedList.Count + 1)))
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    if (getEnumeratorRetType == 2)
                    {
                        return new DictionaryEntry(key, value);
                    }
                    return new KeyValuePair<TKey, TValue>(key, value);
                }
            }
            object IDictionaryEnumerator.Value
            {
                get
                {
                    if ((index == 0) || (index == (_sortedList.Count + 1)))
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return value;
                }
            }
            void IEnumerator.Reset()
            {
                if (version != _sortedList.version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                index = 0;
                key = default(TKey);
                value = default(TValue);
            }
        }


        private sealed class KeyList : IList<TKey>, ICollection
        {
            // Fields
            private readonly SortedList<TKey, TValue> _dict;

            // Methods
            internal KeyList(SortedList<TKey, TValue> dictionary)
            {
                _dict = dictionary;
            }

            public void Add(TKey key)
            {
                ThrowHelper.ThrowNotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public void Clear()
            {
                ThrowHelper.ThrowNotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public bool Contains(TKey key)
            {
                return _dict.ContainsKey(key);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                Array.Copy(_dict.keys, 0, array, arrayIndex, _dict.Count);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return new SortedListKeyEnumerator(_dict);
            }

            public int IndexOf(TKey key)
            {
                if (key == null)
                {
                    ThrowHelper.ThrowArgumentNullException("key");
                }
                int num = Array.BinarySearch(_dict.keys, 0, _dict.Count, key, _dict.comparer);
                if (num >= 0)
                {
                    return num;
                }
                return -1;
            }

            public void Insert(int index, TKey value)
            {
                ThrowHelper.ThrowNotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public bool Remove(TKey key)
            {
                ThrowHelper.ThrowNotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
                return false;
            }

            public void RemoveAt(int index)
            {
                ThrowHelper.ThrowNotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if ((array != null) && (array.Rank != 1))
                {
                    ThrowHelper.ThrowArgumentException("Only single dimensional arrays are supported for the requested action.");
                }
                try
                {
                    Array.Copy(_dict.keys, 0, array, arrayIndex, _dict.Count);
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException("Invalid argument type");
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new SortedListKeyEnumerator(_dict);
            }

            // Properties
            public int Count
            {
                get
                {
                    return _dict._size;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public TKey this[int index]
            {
                get
                {
                    return _dict.GetKey(index);
                }
                set
                {
                    ThrowHelper.ThrowNotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return ((ICollection)_dict).SyncRoot;
                }
            }
        }

        private sealed class ValueList : IList<TValue>, ICollection
        {
            // Fields
            private readonly SortedList<TKey, TValue> _dict;

            // Methods
            internal ValueList(SortedList<TKey, TValue> dictionary)
            {
                _dict = dictionary;
            }

            public void Add(TValue key)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            public void Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            public bool Contains(TValue value)
            {
                return _dict.ContainsValue(value);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                Array.Copy(_dict.values, 0, array, arrayIndex, _dict.Count);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new SortedListValueEnumerator(_dict);
            }

            public int IndexOf(TValue value)
            {
                return Array.IndexOf(_dict.values, value, 0, _dict.Count);
            }

            public void Insert(int index, TValue value)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            public bool Remove(TValue value)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_SortedListNestedWrite);
                return false;
            }

            public void RemoveAt(int index)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if ((array != null) && (array.Rank != 1))
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                try
                {
                    Array.Copy(_dict.values, 0, array, arrayIndex, _dict.Count);
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new SortedListValueEnumerator(_dict);
            }

            // Properties
            public int Count
            {
                get
                {
                    return _dict._size;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public TValue this[int index]
            {
                get
                {
                    return _dict.GetByIndex(index);
                }
                set
                {
                    ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_SortedListNestedWrite);
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return ((ICollection)_dict).SyncRoot;
                }
            }
        }

        private sealed class SortedListKeyEnumerator : IEnumerator<TKey>
        {
            // Fields
            private readonly SortedList<TKey, TValue> _sortedList;
            private TKey currentKey;
            private int index;
            private readonly int version;

            // Methods
            internal SortedListKeyEnumerator(SortedList<TKey, TValue> sortedList)
            {
                _sortedList = sortedList;
                version = sortedList.version;
            }

            public void Dispose()
            {
                index = 0;
                currentKey = default(TKey);
            }

            public bool MoveNext()
            {
                if (version != _sortedList.version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                if (index < _sortedList.Count)
                {
                    currentKey = _sortedList.keys[index];
                    index++;
                    return true;
                }
                index = _sortedList.Count + 1;
                currentKey = default(TKey);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (version != _sortedList.version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                index = 0;
                currentKey = default(TKey);
            }

            // Properties
            public TKey Current
            {
                get
                {
                    return currentKey;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if ((index == 0) || (index == (_sortedList.Count + 1)))
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return currentKey;
                }
            }
        }

        private sealed class SortedListValueEnumerator : IEnumerator<TValue>
        {
            // Fields
            private readonly SortedList<TKey, TValue> _sortedList;
            private TValue currentValue;
            private int index;
            private readonly int version;

            // Methods
            internal SortedListValueEnumerator(SortedList<TKey, TValue> sortedList)
            {
                _sortedList = sortedList;
                version = sortedList.version;
            }

            public void Dispose()
            {
                index = 0;
                currentValue = default(TValue);
            }

            public bool MoveNext()
            {
                if (version != _sortedList.version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                if (index < _sortedList.Count)
                {
                    currentValue = _sortedList.values[index];
                    index++;
                    return true;
                }
                index = _sortedList.Count + 1;
                currentValue = default(TValue);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (version != _sortedList.version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                index = 0;
                currentValue = default(TValue);
            }

            // Properties
            public TValue Current
            {
                get
                {
                    return currentValue;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if ((index == 0) || (index == (_sortedList.Count + 1)))
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return currentValue;
                }
            }
        }

    }
}
