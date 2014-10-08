

using System;
using System.Collections;
using System.Collections.Generic;

namespace ModulusFE
{
    internal static class Algorithms
    {
        /// <summary>
        /// Finds the index of the first item in a list equal to a given item.
        /// </summary>
        /// <remarks>The default sense of equality for T is used, as defined by T's
        /// implementation of IComparable&lt;T&gt;.Equals or object.Equals.</remarks>
        /// <param name="list">The list to search.</param>
        /// <param name="item">The item to search for.</param>
        /// <returns>The index of the first item equal to <paramref name="item"/>. -1 if no such item exists in the list.</returns>
        public static int FirstIndexOf<T>(IList<T> list, T item)
        {
            return FirstIndexOf(list, item, EqualityComparer<T>.Default);
        }

        public static int FirstIndexOf<T>(IEnumerable<T> list, Func<T, bool> predicat)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (predicat == null)
                throw new ArgumentNullException("predicat");

            int index = 0;
            foreach (T x in list)
            {
                if (predicat(x))
                    return index;

                ++index;
            }

            return -1;
        }

        /// <summary>
        /// Finds the index of the first item in a list equal to a given item. A passed
        /// IEqualityComparer is used to determine equality.
        /// </summary>
        /// <param name="list">The list to search.</param>
        /// <param name="item">The item to search for.</param>
        /// <param name="equalityComparer">The IEqualityComparer&lt;T&gt; used to compare items for equality. Only the Equals method will be called.</param>
        /// <returns>The index of the first item equal to <paramref name="item"/>. -1 if no such item exists in the list.</returns>
        public static int FirstIndexOf<T>(IList<T> list, T item, IEqualityComparer<T> equalityComparer)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (equalityComparer == null)
                throw new ArgumentNullException("equalityComparer");

            int index = 0;
            foreach (T x in list)
            {
                if (equalityComparer.Equals(x, item))
                {
                    return index;
                }
                ++index;
            }

            // didn't find any item that matches.
            return -1;
        }

        /// <summary>
        /// Removes all the items in the collection that satisfy the condition
        /// defined by <paramref name="predicate"/>.
        /// </summary>
        /// <remarks>If the collection if an array or implements IList&lt;T&gt;, an efficient algorithm that
        /// compacts items is used. If not, then ICollection&lt;T&gt;.Remove is used
        /// to remove items from the collection. If the collection is an array or fixed-size list,
        /// the non-removed elements are placed, in order, at the beginning of
        /// the list, and the remaining list items are filled with a default value (0 or null).</remarks>
        /// <param name="collection">The collection to check all the items in.</param>
        /// <param name="predicate">A delegate that defines the condition to check for.</param>
        /// <returns>Returns a collection of the items that were removed. This collection contains the
        /// items in the same order that they orginally appeared in <paramref name="collection"/>.</returns>
        public static ICollection<T> RemoveWhere<T>(ICollection<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            if (collection is T[])
                collection = new ArrayWrapper<T>((T[])collection);
            if (collection.IsReadOnly)
                throw new ArgumentException("List is ReadOnly", "collection");

            IList<T> list = collection as IList<T>;
            if (list != null)
            {
                int i = -1, j = 0;
                int listCount = list.Count;
                List<T> removed = new List<T>();

                // Remove item where predicate is true, compressing items to lower in the list. This is much more
                // efficient than the naive algorithm that uses IList<T>.Remove().
                while (j < listCount)
                {
                    T item = list[j];
                    if (predicate(item))
                    {
                        removed.Add(item);
                    }
                    else
                    {
                        ++i;
                        if (i != j)
                            list[i] = item;
                    }
                    ++j;
                }

                ++i;
                if (i < listCount)
                {
                    // remove items from the end.
                    if (list is IList && ((IList)list).IsFixedSize)
                    {
                        // An array or similar. Null out the last elements.
                        while (i < listCount)
                            list[i++] = default(T);
                    }
                    else
                    {
                        // Normal list.
                        while (i < listCount)
                        {
                            list.RemoveAt(listCount - 1);
                            --listCount;
                        }
                    }
                }

                return removed;
            }
            else
            {
                // We have to copy all the items to remove to a List, because collections can't be modifed 
                // during an enumeration.
                List<T> removed = new List<T>();

                foreach (T item in collection)
                    if (predicate(item))
                        removed.Add(item);

                foreach (T item in removed)
                    collection.Remove(item);

                return removed;
            }
        }

        #region Minimum and Maximum

        /// <summary>
        /// Finds the maximum value in a collection.
        /// </summary>
        /// <remarks>Values in the collection are compared by using the IComparable&lt;T&gt;
        /// interfaces implementation on the type T.</remarks>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to search.</param>
        /// <returns>The largest item in the collection. </returns>
        /// <exception cref="InvalidOperationException">The collection is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public static T Maximum<T>(IEnumerable<T> collection)
                where T : IComparable<T>
        {
            return Maximum(collection, Comparer<T>.Default);
        }

        /// <summary>
        /// Finds the maximum value in a collection. A supplied IComparer&lt;T&gt; is used
        /// to compare the items in the collection.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to search.</param>
        /// <param name="comparer">The comparer instance used to compare items in the collection.</param>
        /// <returns>The largest item in the collection.</returns>
        /// <exception cref="InvalidOperationException">The collection is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="comparer"/> is null.</exception>
        public static T Maximum<T>(IEnumerable<T> collection, IComparer<T> comparer)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            T maxSoFar = default(T);
            bool foundOne = false;

            // Go through the collection, keeping the maximum found so far.
            foreach (T item in collection)
            {
                if (!foundOne || comparer.Compare(maxSoFar, item) < 0)
                {
                    maxSoFar = item;
                }

                foundOne = true;
            }

            // If the collection was empty, throw an exception.
            if (!foundOne)
                throw new InvalidOperationException("Collection is empty.");
            return maxSoFar;
        }

        /// <summary>
        /// Finds the maximum value in a collection. A supplied Comparison&lt;T&gt; delegate is used
        /// to compare the items in the collection.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to search.</param>
        /// <param name="comparison">The comparison used to compare items in the collection.</param>
        /// <returns>The largest item in the collection.</returns>
        /// <exception cref="InvalidOperationException">The collection is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="comparison"/> is null.</exception>
        public static T Maximum<T>(IEnumerable<T> collection, Comparison<T> comparison)
        {
            return Maximum(collection, Comparers.ComparerFromComparison(comparison));
        }

        /// <summary>
        /// Finds the minimum value in a collection.
        /// </summary>
        /// <remarks>Values in the collection are compared by using the IComparable&lt;T&gt;
        /// interfaces implementation on the type T.</remarks>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to search.</param>
        /// <returns>The smallest item in the collection.</returns>
        /// <exception cref="InvalidOperationException">The collection is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public static T Minimum<T>(IEnumerable<T> collection)
                where T : IComparable<T>
        {
            return Minimum(collection, Comparer<T>.Default);
        }

        /// <summary>
        /// Finds the minimum value in a collection. A supplied IComparer&lt;T&gt; is used
        /// to compare the items in the collection.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to search.</param>
        /// <param name="comparer">The comparer instance used to compare items in the collection.</param>
        /// <returns>The smallest item in the collection.</returns>
        /// <exception cref="InvalidOperationException">The collection is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="comparer"/> is null.</exception>
        public static T Minimum<T>(IEnumerable<T> collection, IComparer<T> comparer)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            T minSoFar = default(T);
            bool foundOne = false;

            // Go through the collection, keeping the minimum found so far.
            foreach (T item in collection)
            {
                if (!foundOne || comparer.Compare(minSoFar, item) > 0)
                {
                    minSoFar = item;
                }

                foundOne = true;
            }

            // If the collection was empty, throw an exception.
            if (!foundOne)
                throw new InvalidOperationException("Collection is empty.");
            return minSoFar;
        }

        /// <summary>
        /// Finds the minimum value in a collection. A supplied Comparison&lt;T&gt; delegate is used
        /// to compare the items in the collection.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to search.</param>
        /// <param name="comparison">The comparison used to compare items in the collection.</param>
        /// <returns>The smallest item in the collection.</returns>
        /// <exception cref="InvalidOperationException">The collection is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="comparison"/> is null.</exception>
        public static T Minimum<T>(IEnumerable<T> collection, Comparison<T> comparison)
        {
            return Minimum(collection, Comparers.ComparerFromComparison(comparison));
        }
        #endregion
    }

    /// <summary>
    /// The class that is used to implement IList&lt;T&gt; to view an array
    /// in a read-write way. Insertions cause the last item in the array
    /// to fall off, deletions replace the last item with the default value.
    /// </summary>
    class ArrayWrapper<T> : IList<T>
    {
        private readonly T[] wrappedArray;

        /// <summary>
        /// Create a list wrapper object on an array.
        /// </summary>
        /// <param name="wrappedArray">Array to wrap.</param>
        public ArrayWrapper(T[] wrappedArray)
        {
            this.wrappedArray = wrappedArray;
        }

        /// <summary>
        /// Searches the list for the first item that compares equal to <paramref name="item"/>.
        /// If one is found, it is removed. Otherwise, the list is unchanged.
        /// </summary>
        /// <remarks>Equality in the list is determined by the default sense of
        /// equality for T. If T implements IComparable&lt;T&gt;, the
        /// Equals method of that interface is used to determine equality. Otherwise, 
        /// Object.Equals is used to determine equality.</remarks>
        /// <param name="item">The item to remove from the list.</param>
        /// <returns>True if an item was found and removed that compared equal to
        /// <paramref name="item"/>. False if no such item was in the list.</returns>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;
            RemoveAt(index);
            return true;
        }

        public int Count
        {
            get
            {
                return wrappedArray.Length;
            }
        }

        public bool IsReadOnly
        {
            get { return wrappedArray.IsReadOnly; }
        }

        public void Add(T item)
        {
            Insert(Count, item);
        }

        public void Clear()
        {
            int count = wrappedArray.Length;
            for (int i = 0; i < count; ++i)
                wrappedArray[i] = default(T);
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public int IndexOf(T item)
        {
            return Algorithms.FirstIndexOf(this, item, EqualityComparer<T>.Default);
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > wrappedArray.Length)
                throw new ArgumentOutOfRangeException("index");

            if (index + 1 < wrappedArray.Length)
                Array.Copy(wrappedArray, index, wrappedArray, index + 1, wrappedArray.Length - index - 1);
            if (index < wrappedArray.Length)
                wrappedArray[index] = item;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= wrappedArray.Length)
                throw new ArgumentOutOfRangeException("index");

            if (index < wrappedArray.Length - 1)
                Array.Copy(wrappedArray, index + 1, wrappedArray, index, wrappedArray.Length - index - 1);
            wrappedArray[wrappedArray.Length - 1] = default(T);
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= wrappedArray.Length)
                    throw new ArgumentOutOfRangeException("index");

                return wrappedArray[index];
            }
            set
            {
                if (index < 0 || index >= wrappedArray.Length)
                    throw new ArgumentOutOfRangeException("index");

                wrappedArray[index] = value;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (array.Length < wrappedArray.Length)
                throw new ArgumentException("array is too short", "array");
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Length + arrayIndex < wrappedArray.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            Array.Copy(wrappedArray, 0, array, arrayIndex, wrappedArray.Length);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)wrappedArray).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList)wrappedArray).GetEnumerator();
        }

        /// <summary>
        /// Return true, to indicate that the list is fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return true;
            }
        }
    }

    /// <summary>
    /// A collection of methods to create IComparer and IEqualityComparer instances in various ways.
    /// </summary>
    internal static class Comparers
    {
        /// <summary>
        /// Class to change an Comparison&lt;T&gt; to an IComparer&lt;T&gt;.
        /// </summary>
        class ComparisonComparer<T> : IComparer<T>
        {
            private readonly Comparison<T> comparison;

            public ComparisonComparer(Comparison<T> comparison)
            { this.comparison = comparison; }

            public int Compare(T x, T y)
            { return comparison(x, y); }

            public override bool Equals(object obj)
            {
                if (obj is ComparisonComparer<T>)
                    return comparison.Equals(((ComparisonComparer<T>)obj).comparison);
                return false;
            }

            public override int GetHashCode()
            {
                return comparison.GetHashCode();
            }
        }

        /// <summary>
        /// Given an Comparison on a type, returns an IComparer on that type. 
        /// </summary>
        /// <typeparam name="T">T to compare.</typeparam>
        /// <param name="comparison">Comparison delegate on T</param>
        /// <returns>IComparer that uses the comparison.</returns>
        public static IComparer<T> ComparerFromComparison<T>(Comparison<T> comparison)
        {
            if (comparison == null)
                throw new ArgumentNullException("comparison");

            return new ComparisonComparer<T>(comparison);
        }
    }
}
