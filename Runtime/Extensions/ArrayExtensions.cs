using System;
using System.Collections.Generic;

namespace Essentials
{
    public static class ArrayExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> collection, T search)
        {
            var index = 0;
            foreach (var element in collection)
            {
                if (Equals(element, search)) return index;
                index++;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the first entry matching the search function, or -1 if no match is found.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="search"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> search)
        {
            var index = 0;
            foreach (var element in collection)
            {
                if (search(element)) return index;
                index++;
            }

            return -1;
        }

        public static int IndexOfMin(this IList<int> self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (self.Count == 0)
            {
                throw new ArgumentException("List is empty.", nameof(self));
            }

            var min = self[0];
            var minIndex = 0;

            for (var i = 1; i < self.Count; ++i)
            {
                if (self[i] < min)
                {
                    min = self[i];
                    minIndex = i;
                }
            }

            return minIndex;
        }

        public static int IndexOfMax(this IList<int> self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (self.Count == 0)
            {
                throw new ArgumentException("List is empty.", nameof(self));
            }

            var max = self[0];
            var maxIndex = 0;

            for (var i = 1; i < self.Count; ++i)
            {
                if (self[i] > max)
                {
                    max = self[i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        public static int IndexOfMin(this IList<float> self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (self.Count == 0)
            {
                throw new ArgumentException("List is empty.", nameof(self));
            }

            var min = self[0];
            var minIndex = 0;

            for (var i = 1; i < self.Count; ++i)
            {
                if (self[i] < min)
                {
                    min = self[i];
                    minIndex = i;
                }
            }

            return minIndex;
        }

        public static int IndexOfMax(this IList<float> self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (self.Count == 0)
            {
                throw new ArgumentException("List is empty.", nameof(self));
            }

            var max = self[0];
            var maxIndex = 0;

            for (var i = 1; i < self.Count; ++i)
            {
                if (self[i] > max)
                {
                    max = self[i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }
    }
}