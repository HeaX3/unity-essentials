using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Essentials
{
    public static class LinqExtensions
    {
        private static readonly Random RandomInstance = new Random();

        public static Vector3 Average(this IEnumerable<Vector3> collection)
        {
            var x = 0f;
            var y = 0f;
            var z = 0f;
            var count = 0;
            foreach (var vector in collection)
            {
                x += vector.x;
                y += vector.y;
                z += vector.z;
                count++;
            }

            return count > 0 ? new Vector3(x / count, y / count, z / count) : Vector3.zero;
        }

        public static T Random<T>(this IEnumerable<T> collection)
        {
            return collection.Random(i => 1, RandomInstance);
        }

        public static T Random<T>(this IEnumerable<T> collection, Random random)
        {
            return collection.Random(i => 1, random);
        }

        public static T Random<T>(this IEnumerable<T> collection, Func<T, double> weightKey)
        {
            return collection.Random(weightKey, RandomInstance);
        }

        public static T Random<T>(this IEnumerable<T> collection, Func<T, double> weightKey, Random random)
        {
            var items = collection.ToList();

            var totalWeight = items.Sum(weightKey);
            var randomWeightedIndex = random.NextDouble() * totalWeight;
            var itemWeightedIndex = 0d;
            foreach (var item in items)
            {
                itemWeightedIndex += weightKey(item);
                if (randomWeightedIndex < itemWeightedIndex)
                    return item;
            }

            throw new ArgumentException("Collection count and weights must be greater than 0");
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}